using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Amino
{
	/// <summary> A component which adds a rendered <see cref="Texture2D"/> sprite to its entity. </summary>
	public class SpriteComponent : Component
    {
		/// <summary> The texture the sprite renders. </summary>
		protected Texture2D _texture;
		/// <summary> The batch used when rendering the sprite. </summary>
		private SpriteBatch _spriteBatch;
		/// <summary> How many pixels of the sprite's texture fit into one game unit at default scale. </summary>
		private float _pixelsPerUnit;

		/// <summary> The camera used to render the sprite. </summary>
		protected Camera Camera => Owner.World.Camera;

		/// <summary> The color used to tint the sprite. </summary>
		public Color Color { get; set; } = Color.White;

		/// <summary>
		/// The location of this sprite's pivot point, or the point on its texture that its origin will sit.
		/// If (0.5, 0.5), the sprite will have the origin at its centre.
		/// </summary>
		public Vector2 Offset
		{
			get => _offset;
			set
			{
				if(value == _offset)
				{
					return;
				}
				_offset = value;
				OffsetType = value.ToAnchorType();
			}
		}
		private Vector2 _offset;

		/// <summary>
		/// The location of this sprite's pivot point, or the point on its texture that its origin will sit.
		/// </summary>
		public AnchorType OffsetType
		{
			get => _offsetType;
			set
			{
				if (value == _offsetType)
				{
					return;
				}
				_offsetType = value;
				_offset = value.ToVector();
			}
		}
		private AnchorType _offsetType;

		/// <summary>
		/// Construct a <see cref="SpriteComponent"/> with the default sprite.
		/// To use this, a sprite must have been specified as the <see cref="Config.DefaultSprite"/>.
		/// </summary>
		public SpriteComponent(Entity owner) : base(owner)
		{
			if(Config.DefaultSprite == null)
			{
				throw new InvalidOperationException($"Cannot call {nameof(SpriteComponent)} constructor with unspecified sprite: Specify a default sprite in {nameof(Config.DefaultSprite)}.");
			}
			Init(World.Content.Load<Texture2D>(Config.DefaultSprite));
		}

        public SpriteComponent(Entity owner, string spriteName) : base(owner)
        {
            Init(World.Content.Load<Texture2D>(spriteName));
        }

        public SpriteComponent(Entity owner, Texture2D texture) : base(owner)
        {
            Init(texture);
        }

		private void Init(Texture2D texture)
        {
			OffsetType = AnchorType.Centre;
			_texture = texture;

			World.Drawing += Draw;
            _spriteBatch = new SpriteBatch(World.GraphicsDevice);
			_pixelsPerUnit = Config.DefaultPixelsPerUnit;
		}

        private void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			Matrix3x3 renderTransform = Camera.RenderTransform * Owner.Transform;
			Vector2 pixelPerUnitScale = new Vector2(1f / _pixelsPerUnit, 1f / _pixelsPerUnit);

			_spriteBatch.Draw(
                _texture,
				renderTransform.Translation,
                null,
                Color,
				renderTransform.Rotation,
                new Vector2(Offset.X * _texture.Width, Offset.Y * _texture.Height),
				renderTransform.Scale * pixelPerUnitScale,
                SpriteEffects.None,
                0f);

			_spriteBatch.End();
        }
    }
}
