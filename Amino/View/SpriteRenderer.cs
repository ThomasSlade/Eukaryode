using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary> Renders a <see cref="Sprite"/>. </summary>
	public class SpriteRenderer : EntityRenderer
	{
		/// <summary> The <see cref="Sprite"/> that this renderer draws. </summary>
		private Sprite _sprite;
		/// <summary> The texture of the <see cref="_sprite"/>, as defined by its <see cref="Sprite._textureKey"/>. </summary>
		private Texture2D _texture;

		/// <summary> The spatial offseet that this sprite renderer should draw at, given its <see cref="Sprite.OffsetFactor"/> and <see cref="_texture"/> dimensions. </summary>
		public Vector2 Offset => new Vector2(_sprite.OffsetFactor.X * _texture.Width, _sprite.OffsetFactor.Y * _texture.Height);

		public SpriteRenderer(RenderService renderer, Sprite sprite) : base(renderer)
		{
			_sprite = sprite;
			_texture = _renderer.Content.Load<Texture2D>(_sprite.TextureKey);
		}

		public override void Draw(GameTime gameTime, ref SpriteBatch batch, Matrix3x3 cameraMatrix)
		{
			Matrix3x3 renderTransform = cameraMatrix * _sprite.Owner.Transform;

			batch.Draw(
				_texture,
				renderTransform.Translation,
				null,
				_sprite.Color,
				renderTransform.Rotation,
				Offset,
				renderTransform.Scale * _sprite.PixelScale,
				SpriteEffects.None,
				0f);
		}
	}
}
