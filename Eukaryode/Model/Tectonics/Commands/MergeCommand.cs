using Microsoft.Xna.Framework;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// A command to combine the characteristics of several <see cref="FlowPoint"/>s which converge on the same cell.
	/// </summary>
	public class MergeCommand : Command
	{
		public Vector2Int[] _toMerge;

		public MergeCommand(Vector2Int target, Vector2Int[] toMerge) : base(target)
		{
			_toMerge = toMerge;
		}

		public override void Execute(FlowPoint[,] currentGrid, FlowPoint[,] newGrid)
		{
			FlowPoint[] toMerge = new FlowPoint[_toMerge.Length];
			for (int m = 0; m < _toMerge.Length; m++)
			{
				Vector2Int mergeCoord = _toMerge[m];
				toMerge[m] = currentGrid[mergeCoord.X, mergeCoord.Y];
			}
			newGrid[Target.X, Target.Y] = new FlowPoint(toMerge, true);
		}
	}
}
