using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Amino
{
	/// <summary> The parameters for drawing a debug line, with positions representing world space. </summary>
	public struct DebugLine : IDebugRenderRequest
	{
		private Matrix3x3 From { get; init; }
		private Matrix3x3 To { get; init; }
		private Color Color { get; init; }
		private float Thickness { get; init; }

		public DebugLine(Vector2 from, Vector2 to, Color color, float thickness = 1f)
		{
			From = Matrix3x3.TranslationMatrix(from);
			To = Matrix3x3.TranslationMatrix(to);
			Color = color;
			Thickness = thickness;
		}

		public void Draw(SpriteBatch spriteBatch, Matrix3x3 cameraMatrix)
		{
			spriteBatch.DrawLine((cameraMatrix * From).Translation, (cameraMatrix * To).Translation, Color, Thickness);
		}
	}
}
