using Microsoft.Xna.Framework;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// A command to move a <see cref="FlowPoint"/> from one cell to another.
	/// </summary>
	public class MoveCommand : Command
	{
		public Vector2Int _source;

		public MoveCommand(Vector2Int target, Vector2Int source) : base(target)
		{
			_source = source;
		}

		public override void Execute(FlowPoint[,] currentGrid, FlowPoint[,] newGrid)
		{
			newGrid[Target.X, Target.Y] = currentGrid[_source.X, _source.Y];
		}
	}
}
