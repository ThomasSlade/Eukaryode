using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary> Renders something within the context of the <see cref="RenderService"/>. </summary>
	public abstract class EntityRenderer
	{
		/// <summary> The <see cref="RenderService"/> that controls this entity renderer. </summary>
		protected RenderService _renderer;

		public EntityRenderer(RenderService renderer)
		{
			_renderer = renderer;
		}

		/// <summary> Draw this renderer's contents. This typically involves making a call to a <see cref="SpriteBatch"/> </summary>
		public abstract void Draw(GameTime gameTime, ref SpriteBatch batch, Matrix3x3 cameraMatrix);
	}
}
