using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// A space on the tectonic map, which can be occupied by exactly one <see cref="FlowPoint"/>.
	/// </summary>
	public class FlowCell
	{
		/// <summary> This cell's coordinates within the grid. </summary>
		public Vector2Int Coordinates { get; private init; }

		/// <summary> The <see cref="FlowPoint"/> currently occupying this cell. </summary>
		public FlowPoint Occupant
		{
			get => _occupant;
			set => _occupant = value;
		}
		private FlowPoint _occupant;

		/// <summary> The vector by which this cell's occupant will move. </summary>
		public Vector2 Movement
		{
			get => _occupant.Movement;
			set => _occupant.Movement = value;
		}

		/// <summary> The cells whose occupants are moving into this cell. </summary>
		public HashSet<FlowCell> Incoming { get; private init; } = new HashSet<FlowCell>(2);

		public FlowCell(Vector2Int coordinates, FlowPoint initialOccupant)
		{
			Coordinates = coordinates;
			Occupant = initialOccupant;
		}

		/// <summary> Move this cell's occupant by its movement vector. </summary>
		public void Move()
		{
			_occupant.Position += Occupant.Movement;
			_occupant.Movement = Vector2.Zero;
		}
	}
}
