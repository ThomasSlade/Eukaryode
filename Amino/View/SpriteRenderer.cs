﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary> Renders a <see cref="Sprite"/>. </summary>
	public class SpriteRenderer
	{
		/// <summary> The <see cref="Renderer"/> that controls this sprite. </summary>
		private Renderer _renderer;
		/// <summary> The <see cref="Sprite"/> that this renderer draws. </summary>
		private Sprite _sprite;
		/// <summary> The texture of the <see cref="_sprite"/>, as defined by its <see cref="Sprite._textureKey"/>. </summary>
		private Texture2D _texture;
		/// <summary> The <see cref="SpriteBatch"/> being used to render. </summary>
		private SpriteBatch _spriteBatch;

		/// <summary> The spatial offseet that this sprite renderer should draw at, given its <see cref="Sprite.OffsetFactor"/> and <see cref="_texture"/> dimensions. </summary>
		public Vector2 Offset => new Vector2(_sprite.OffsetFactor.X * _texture.Width, _sprite.OffsetFactor.Y * _texture.Height);

		public SpriteRenderer(Renderer renderer, Sprite sprite)
		{
			_renderer = renderer;
			_sprite = sprite;
			_texture = _renderer.Content.Load<Texture2D>(_sprite.TextureKey);
			_spriteBatch = new SpriteBatch(_renderer.GraphicsDevice);
		}

		public void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			Matrix3x3 renderTransform = _renderer.CameraRenderTransform * _sprite.Owner.Transform;

			Vector2 testOffset = _renderer._cameraScreenMatrix * _sprite.OffsetFactor;

			_spriteBatch.Draw(
				_texture,
				renderTransform.Translation,
				null,
				_sprite.Color,
				renderTransform.Rotation,
				Offset,
				renderTransform.Scale * _sprite.PixelScale,
				SpriteEffects.None,
				0f);

			_spriteBatch.End();
		}
	}
}
