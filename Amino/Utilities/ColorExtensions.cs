using Microsoft.Xna.Framework;
using DrawingCol = System.Drawing.Color;

namespace Amino
{
	/// <summary> Extensions relating to assorted color classes. </summary>
	public static class ColorExtensions
	{
		public static Color ToXNACol(this DrawingCol dCol)
		{
			return new Color(dCol.R, dCol.G, dCol.B, dCol.A);
		}

		public static DrawingCol ToDrawingCol(this Color col)
		{
			return DrawingCol.FromArgb(col.A, col.R, col.G, col.B);
		}
	}
}
