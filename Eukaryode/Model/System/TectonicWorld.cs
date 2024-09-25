using Amino;
using Eukaryode.Tectonics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eukaryode
{
	/// <summary> Defines the initial state of a world's map and its progression over time. </summary>
	public class TectonicWorld : Component
	{
		/// <summary> The geological time that had passed at the last tectonic tick. </summary>
		private float currentTime = 0f;

		/// <summary> The sequence of snapshots this tectonic world will interpolate between. </summary>
		private List<TectonicSnapshot> _snapshots = new();

		/// <summary> The snapshot we're interpolating from. </summary>
		public TectonicSnapshot _currentSnapshot;
		/// <summary> The snapshot we're interpolating to. </summary>
		private TectonicSnapshot? _targetSnapshot;

		/// <summary>
		/// The grid used to model the flow of cell values as the world shifts from one tectonic snapshot to another.
		/// </summary>
		/// <remarks>
		/// The flow grid is composed of <see cref="FlowPoint"/>s: at the start of any tectonic tick, there will be only one point within the
		/// bounds of a cell. These points move to try and reach their destination on the target cell, and the grid performs merge/split operations
		/// to maintain 1-point-per-cell.
		/// </remarks>
		private FlowCell[,] _flowGrid;

		/// <summary> The actual gameplay grid, which this object drives the movement of. </summary>
		private LayerGrid _layerGrid;

		/// <summary> The cell width of the world map. </summary>
		private int Width { get; init; }
		/// <summary> The cell height of the world map. </summary>
		private int Height { get; init; }

		public TectonicWorld(Entity owner, TectonicData data, IList<TectonicSnapshot> tectonicSnapshots) : base(owner)
		{
			_snapshots = tectonicSnapshots.ToList();
			_currentSnapshot = _snapshots.First();
			Width = _currentSnapshot.Width;
			Height = _currentSnapshot.Height;

			if (_snapshots.Count > 1)
			{
				_targetSnapshot = _snapshots[1];
				_flowGrid = CreateFlowGrid(_currentSnapshot, _targetSnapshot);
			}
			else
			{
				throw new InvalidOperationException("At least two tectonic snapshots are needed.");
			}

			_layerGrid = CreateWorld();
		}

		private FlowCell[,] CreateFlowGrid(TectonicSnapshot currentSnapshot, TectonicSnapshot targetSnapshot)
		{
			FlowPoint[,] newTiles = InitFlowGridTargets(currentSnapshot, targetSnapshot);
			FlowCell[,] newFlowGrid = new FlowCell[Width, Height];
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					newFlowGrid[x, y] = new FlowCell(new Vector2Int(x, y), newTiles[x, y]);
				}
			}
			return newFlowGrid;
		}

		/// <summary>
		/// Set the flow tiles for the grid, by creating a tile for each grid square and then determining
		/// where it wants to move given the next snapshot.
		/// </summary>
		private FlowPoint[,] InitFlowGridTargets(TectonicSnapshot currentSnapshot, TectonicSnapshot targetSnapshot)
		{
			FlowPoint[,] initialisedTiles = new FlowPoint[Width, Height];

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					initialisedTiles[x, y] = currentSnapshot.GetTile(x, y, targetSnapshot);
				}
			}

			return initialisedTiles;
		}

		private LayerGrid CreateWorld()
		{
			Entity gridEntity = new Entity(World);
			LayerGrid grid = LayerGrid.Create(gridEntity, Width, Height);

			for (int x = 0; x < grid.Width; x++)
			{
				for (int y = 0; y < grid.Height; y++)
				{
					// Image coordinate origin is top-left, but grid is bottom-left, so need to reverse the y coord.
					int pixelIndex = grid.Width * (grid.Height - 1 - y) + x;
					grid[x, y].SurfaceAltitude = _currentSnapshot.GetAltitude(new Vector2Int(x, y));
				}
			}

			return grid;
		}

		/// <summary>
		/// The algorithm for making an interpolation step between the <see cref="_currentSnapshot"/> and <see cref="_targetSnapshot"/>.
		/// </summary>
		/// <param name="millionsOfYears"> The millions of years which have passed since the last tick. Clamped to remaining geological time between the snapshots.</param>
		/// <remarks>
		/// The algorithm works like this:
		/// 1. Compute point movements for all points in the <see cref="_flowGrid"/> towards their target cell.
		/// 2. Determine which cells need to split, merge, and move in order to keep the grid one-point-per-cell after this movement is executed.
		/// 3. Execute these split, merge, and move commands.
		/// 4. Move the points by the vectors determined in step 1.
		/// 5. Interpolate the altitudes between each point and its target.
		/// </remarks>
		public void TectonicTick(float millionsOfYears)
		{
			float remainingTime = _targetSnapshot.GeologicalTime - currentTime;
			if (remainingTime == 0f)
			{
				return;
			}
			currentTime += millionsOfYears;

			float lerp = Math.Clamp(millionsOfYears / remainingTime, 0f, 1f);

			ComputePointMovements(lerp);
			List<Command> tileCommands = DetermineCommands();
			ExecuteCommands(tileCommands);
			MovePoints();
			LerpAltitudes(lerp);
		}

		/// <summary>
		/// Determine the <see cref="FlowPoint.Movement"/> values for each point.
		/// </summary>
		private void ComputePointMovements(float lerp)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					_flowGrid[x, y].Movement = (_flowGrid[x, y].Occupant.Destination - _flowGrid[x, y].Occupant.Position) * lerp;
				}
			}
		}

		/// <summary>
		/// Apply the <see cref="FlowPoint.Movement"/> values to each point.
		/// </summary>
		private void MovePoints()
		{
			int pointsSkippingNeighborsCount = 0;   // Count the cells moving to a cell which isn't in their moore neighborhood.
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if ((_flowGrid[x, y].Occupant.TargetCell - _flowGrid[x, y].Occupant.PositionCell).Length() > Math.Sqrt(2f))
					{
						pointsSkippingNeighborsCount++;

					}
					_flowGrid[x, y].Move();
				}
			}

			if (pointsSkippingNeighborsCount > 0)
			{
				System.Diagnostics.Debug.WriteLine(
					$"Warning: {pointsSkippingNeighborsCount} moved greater than the distance of a single cell this tectonic update. The results of the tectonic lerp may not match the target snapshot."
				);
			}
		}

		/// <summary>
		/// Determine <see cref="Command"/>s needed to keep the <see cref="_flowGrid"/> at one-point-per-cell given
		/// the current movement values of all flow points.
		/// </summary>
		private List<Command> DetermineCommands()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					_flowGrid[x, y].Incoming.Clear();
				}
			}

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2Int targetCell = _flowGrid[x, y].Occupant.TargetCell;
					_flowGrid[targetCell.X, targetCell.Y].Incoming.Add(_flowGrid[x, y]);
				}
			}

			List<Command> moveCommands = new List<Command>();
			List<Command> fillCommands = new List<Command>();
			List<Command> mergeCommands = new List<Command>();

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (_flowGrid[x, y].Incoming.Count > 1)
					{
						mergeCommands.Add(new MergeCommand(_flowGrid[x, y].Coordinates, _flowGrid[x, y].Incoming.Select(i => i.Coordinates).ToArray()));
					}
					else if (_flowGrid[x, y].Incoming.Count == 1)
					{
						moveCommands.Add(new MoveCommand(_flowGrid[x, y].Coordinates, _flowGrid[x, y].Incoming.First().Coordinates));
					}
					else
					{
						fillCommands.Add(new SplitCommand(_flowGrid[x, y].Coordinates));
					}
				}
			}

			moveCommands.AddRange(fillCommands);
			moveCommands.AddRange(mergeCommands);

			return moveCommands;
		}

		/// <summary>
		/// Execute the given list of commands.
		/// </summary>
		private void ExecuteCommands(List<Command> commands)
		{
			FlowPoint[,] existingTiles = new FlowPoint[Width, Height];
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					existingTiles[x, y] = _flowGrid[x, y].Occupant;
				}
			}
			FlowPoint[,] newTiles = new FlowPoint[Width, Height];

			foreach (Command command in commands)
			{
				command.Execute(existingTiles, newTiles);
			}

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					_flowGrid[x, y].Occupant = newTiles[x, y];
				}
			}
		}

		/// <summary>
		/// Interpolate the altitudes of all <see cref="FlowPoint"/>s in the <see cref="_flowGrid"/> towards that of the <see cref="_targetSnapshot"/>.
		/// </summary>
		private void LerpAltitudes(float lerp)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					FlowPoint tile = _flowGrid[x, y].Occupant;
					float diff = _targetSnapshot.GetAltitude(tile.DestinationCell) - _flowGrid[x, y].Occupant.CurrentAltitude;
					tile.CurrentAltitude += diff * lerp;
					_flowGrid[x, y].Occupant = tile;

					_layerGrid[x, y].SurfaceAltitude = tile.CurrentAltitude;
				}
			}
		}
	}
}
