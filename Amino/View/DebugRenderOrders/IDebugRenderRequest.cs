using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary> Signifies that an object represents the order to render a debug-shape. </summary>
	public interface IDebugRenderRequest
	{
		/// <summary> Invoke the appropriate behaviour on the provided batch in order to draw this object. </summary>
		public void Draw(SpriteBatch spriteBatch, Matrix3x3 cameraMatrix);
	}
}
