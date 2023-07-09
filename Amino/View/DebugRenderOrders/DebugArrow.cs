using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Amino
{
	/// <summary> The parameters for drawing an arrow, with positions in world space. </summary>
	public struct DebugArrow : IDebugRenderRequest
	{
		private Matrix3x3 From { get; init; }
		private Matrix3x3 To { get; init; }
		private Color Color { get; init; }
		private float HeadMagnitude { get; init; }
		private float Thickness { get; init; }

		public DebugArrow(Vector2 from, Vector2 to, Color color, float headWidth = 0.1f, float thickness = 1f)
		{
			From = Matrix3x3.TranslationMatrix(from);
			To = Matrix3x3.TranslationMatrix(to);
			Color = color;
			// Roughly equal to the adjacent/opposite of a right triangle with hypotenuse of 1, thus how long an arrow flank should be to match the head width.
			HeadMagnitude = headWidth * 0.707f;
			Thickness = thickness;
		}

		public void Draw(SpriteBatch spriteBatch, Matrix3x3 cameraMatrix)
		{
			Vector2 fromPoint = From.Translation;
			Vector2 toPoint = To.Translation;
			Vector2 diff = fromPoint - toPoint;

			Vector2 leftFlank = diff.Rotate(MathHelper.ToRadians(-45f)).NormalizedCopy() * HeadMagnitude;
			Vector2 rightFlank = diff.Rotate(MathHelper.ToRadians(45f)).NormalizedCopy() * HeadMagnitude;

			spriteBatch.DrawLine((cameraMatrix * From).Translation, (cameraMatrix * To).Translation, Color, Thickness);
			spriteBatch.DrawLine((cameraMatrix * From).Translation, (cameraMatrix * To).Translation, Color, Thickness);
			spriteBatch.DrawLine((cameraMatrix * To).Translation,
				(cameraMatrix * (Matrix3x3.TranslationMatrix(toPoint + leftFlank))).Translation, Color, Thickness);
			spriteBatch.DrawLine((cameraMatrix * To).Translation,
				(cameraMatrix * (Matrix3x3.TranslationMatrix(toPoint + rightFlank))).Translation, Color, Thickness);
		}
	}
}
