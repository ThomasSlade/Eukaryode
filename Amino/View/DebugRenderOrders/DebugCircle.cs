using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Amino
{
	/// <summary> Parameters for drawing a circle, with position in world space. </summary>
	public struct DebugCircle : IDebugRenderRequest
	{
		private Matrix3x3 _centre;
		private float _radius;
		private Color _color;
		private int _sides;
		private float _thickness;

		public DebugCircle(Vector2 centre, float radius, Color color, int sides = 16, float thickness = 1f)
		{
			_centre = Matrix3x3.TranslationMatrix(centre);
			_radius = radius;
			_color = color;
			_sides = sides;
			_thickness = thickness;
		}

		public void Draw(SpriteBatch spriteBatch, Matrix3x3 cameraMatrix)
		{
			spriteBatch.DrawCircle((cameraMatrix * _centre).Translation, _radius, _sides, _color, _thickness);
		}
	}
}
