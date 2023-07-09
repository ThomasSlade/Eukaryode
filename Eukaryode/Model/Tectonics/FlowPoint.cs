using Amino;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// A single point on the tectonic map, which tracks a position on the flow grid and a movement vector.
	/// At the start of each tectonic tick, there will be exactly one <see cref="FlowPoint"/> within each <see cref="FlowCell"/>.
	/// As these points move during the tick, they are merged and split in order to keep each cell occupied by only one point.
	/// </summary>
	public struct FlowPoint
	{
		/// <summary> A randomised colour that can be used for debugging the point. </summary>
		public Color DebugColor { get; init; } = default;

		/// <summary> The current spatial location of this point on the grid. </summary>
		public Vector2 Position
		{
			get => _position;
			set
			{
				_position = value;
				value.Floor();
				PositionCell = (Vector2Int)value;
			}
		}
		private Vector2 _position = default;
		/// <summary> The cell this point currently occupies. This is driven by <see cref="Position"/>. </summary>
		public Vector2Int PositionCell { get; private set; } = default;

		/// <summary>
		/// The spatial location this point is trying to move towards this tectonic interpolation. This is always the centre of a cell (e.g. 0.5, 0.5).
		/// Driven by <see cref="Destination"/>.
		/// </summary>
		public Vector2 Destination { get; private set; } = default;
		/// <summary> The cell this point is trying to move towards this tectonic interpolation. </summary>
		public Vector2Int DestinationCell
		{
			get => _destinationCell;
			set
			{
				_destinationCell = value;
				Destination = ((Vector2)value) + Vector2.One * 0.5f;
			}
		}
		private Vector2Int _destinationCell = default;

		/// <summary> The vector by which this point will move within this tectonic tick. </summary>
		public Vector2 Movement { get; set; } = default;
		/// <summary> The position this point is going to move to next, once <see cref="Movement"/> is applied to <see cref="Position"/>. </summary>
		public Vector2 TargetLocation => Position + Movement;
		/// <summary> The cell this point will move to next, once <see cref="Movement"/> is applied to <see cref="Position"/>. </summary>
		public Vector2Int TargetCell
		{
			get
			{
				Vector2 t = TargetLocation;
				t.Floor();
				return (Vector2Int)t;
			}
		}

		/// <summary> The current terrain altitude this cell has. This is applied to the map to raise and lower mountains. </summary>
		public float CurrentAltitude { get; set; } = 0f;
		/// <summary> The triangle this point belongs to this tectonic iteration. </summary>
		public PlateTriangle Triangle { get; init; } = default;

		public FlowPoint(Vector2 currentLocation, Vector2Int destinationCell, float currentAltitude, PlateTriangle triangle)
		{
			Position = currentLocation;
			DestinationCell = destinationCell;
			CurrentAltitude = currentAltitude;
			Triangle = triangle;
			DebugColor = AminoUtils.RandomColor();
		}

		/// <summary>
		/// Construct a <see cref="FlowPoint"/> by averaging the values of a set of other points.
		/// </summary>
		/// <param name="mergeMovementVectors"> If false, the resulting point will have no movement vector. </param>
		public FlowPoint(FlowPoint[] toMerge, bool mergeMovementVectors)
		{
			if(toMerge.Length == 0)
			{
				throw new ArgumentException(nameof(toMerge));
			}

			Vector2 avgCurrentLocation = Vector2.Zero;
			Vector2 avgTargetLocation = Vector2.Zero;
			Vector2 avgMovement = Vector2.Zero;
			CurrentAltitude = 0f;
			Dictionary<PlateTriangle, int> triangleCounts = new Dictionary<PlateTriangle, int>();
			PlateTriangle? mostFrequentTriangle = null;

			for (int m = 0; m < toMerge.Length; m++)
			{
				FlowPoint mergeTile = toMerge[m];
				avgCurrentLocation += mergeTile.Position;
				avgTargetLocation += mergeTile.Destination;
				avgMovement += mergeTile.Movement;
				CurrentAltitude += mergeTile.CurrentAltitude;
				triangleCounts.TryAdd(mergeTile.Triangle, 0);
				triangleCounts[mergeTile.Triangle]++;
				if(!mostFrequentTriangle.HasValue || triangleCounts[mergeTile.Triangle] > triangleCounts[mostFrequentTriangle.Value])
				{
					mostFrequentTriangle = mergeTile.Triangle;
				}
			}

			avgCurrentLocation /= toMerge.Length;
			avgTargetLocation /= toMerge.Length;
			avgTargetLocation.Floor();
			avgMovement /= toMerge.Length;
			Position = avgCurrentLocation;
			DestinationCell = (Vector2Int)avgTargetLocation;
			CurrentAltitude /= toMerge.Length;

			if(mergeMovementVectors)
			{
				Movement = avgMovement;
			}

			Triangle = mostFrequentTriangle.Value;
			DebugColor = AminoUtils.RandomColor();
		}
	}
}
