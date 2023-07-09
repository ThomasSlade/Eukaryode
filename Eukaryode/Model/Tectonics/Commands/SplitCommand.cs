using Amino;
using Microsoft.Xna.Framework;

namespace Eukaryode.Tectonics
{
	/// <summary>
	/// A command to split a <see cref="FlowPoint"/> departing from its cell, leaving behind a new point which
	/// is a merge between that point and the point opposite its departure vector.
	/// </summary>
	public class SplitCommand : Command
	{
		public SplitCommand(Vector2Int target) : base(target)
		{
		}

		public override void Execute(FlowPoint[,] currentGrid, FlowPoint[,] newGrid)
		{
			FlowPoint departingPoint = currentGrid[Target.X, Target.Y];

			Vector2Int cellMovementVector = departingPoint.TargetCell - departingPoint.PositionCell;
			Vector2Int opposingCell = departingPoint.PositionCell - cellMovementVector;
			opposingCell.X = MathUtils.Mod(opposingCell.X, currentGrid.GetLength(0));

			if (opposingCell.Y < 0 || opposingCell.Y >= currentGrid.GetLength(1))
			{
				newGrid[Target.X, Target.Y] = departingPoint;
				newGrid[Target.X, Target.Y].Movement = Vector2.Zero;
			}
			else
			{
				FlowPoint opposintPoint = currentGrid[opposingCell.X, opposingCell.Y];

				FlowPoint departingPointTarget = new FlowPoint(
					departingPoint.TargetLocation,
					departingPoint.DestinationCell,
					departingPoint.CurrentAltitude,
					departingPoint.Triangle);

				newGrid[Target.X, Target.Y] = new FlowPoint(new FlowPoint[] { departingPointTarget, opposintPoint }, false);
			}
		}
	}
}
