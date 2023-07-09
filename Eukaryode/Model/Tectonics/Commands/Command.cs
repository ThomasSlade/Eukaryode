using Microsoft.Xna.Framework;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// Represents a command to the <see cref="TectonicWorld"/> map to adjust its tiles in some way.
	/// </summary>
	public abstract class Command
	{
		/// <summary> The cell this command will produce an occupant for. </summary>
		public Vector2Int Target { get; private init; }

		public Command(Vector2Int target)
		{
			Target = target;
		}

		public abstract void Execute(FlowPoint[,] currentGrid, FlowPoint[,] newGrid);
	}
}
