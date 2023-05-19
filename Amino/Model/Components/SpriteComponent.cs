using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Amino
{
	/// <summary> A component which adds a rendered <see cref="Texture2D"/> sprite to its entity. </summary>
	public class SpriteComponent : Component
    {
		/// <summary> The key of the texture the sprite renders. </summary>
		public string TextureKey => _textureKey;
		protected string _textureKey;

		/// <summary> How many pixels of the sprite's texture fit into one game unit at default scale. </summary>
		public float PixelsPerUnit
		{
			get => _pixelsPerUnit;
			set
			{
				if(value == _pixelsPerUnit)
				{
					return;
				}
				_pixelsPerUnit = value;
				_pixelScale = new Vector2(1f / _pixelsPerUnit, 1f / _pixelsPerUnit);
			}
		}
		private float _pixelsPerUnit;

		/// <summary> The width and height each pixel of this sprite occupies, given its pixels per unit. </summary>
		public Vector2 PixelScale => _pixelScale;
		private Vector2 _pixelScale = Vector2.One;

		/// <summary> The camera used to render the sprite. </summary>
		protected CameraComponent Camera => Owner.World.Camera;

		/// <summary> The color used to tint the sprite. </summary>
		public Color Color { get; set; } = Color.White;

		/// <summary>
		/// The location of this sprite's pivot point, or the point on its texture that its origin will sit.
		/// If (0.5, 0.5), the sprite will have the origin at its centre.
		/// </summary>
		public Vector2 OffsetFactor
		{
			get => _offsetFactor;
			set
			{
				if(value == _offsetFactor)
				{
					return;
				}
				_offsetFactor = value;
				OffsetType = value.ToAnchorType();
			}
		}
		private Vector2 _offsetFactor;

		/// <summary> The location of this sprite's pivot point, or the point on its texture that its origin will sit. </summary>
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
				_offsetFactor = value.ToVector();
			}
		}
		private AnchorType _offsetType;

		/// <summary> Get the matrix that can be used to transform this sprite's position into screenspace. </summary>
		//public Matrix3x3 RenderTransform => Camera.RenderTransform * Owner.Transform;

		/// <summary>
		/// Create a <see cref="SpriteComponent"/>.
		/// </summary>
		/// <param name="textureKey">If unspecified, <see cref="Config.DefaultSprite"/> will be used.</param>
		public static SpriteComponent Create(Entity owner, string? textureKey = null)
		{
			SpriteComponent c = new SpriteComponent(owner, textureKey);
			owner.World.OnComponentCreated(c);
			return c;
		}

		protected SpriteComponent(Entity owner, string? textureKey = null) : base(owner)
        {
			if(textureKey == null)
			{
				if (Config.DefaultSprite == null)
				{
					throw new InvalidOperationException($"Cannot call {nameof(SpriteComponent)} constructor with unspecified sprite key: Specify a default sprite in {nameof(Config.DefaultSprite)}.");
				}
				textureKey = Config.DefaultSprite;
			}
			
			_textureKey = textureKey;
			OffsetType = AnchorType.Centre;
			PixelsPerUnit = Config.DefaultPixelsPerUnit;
		}
    }
}
