using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Amino
{
	/// <summary> A camera used to position the rendering viewport. </summary>
	public class Camera : Component
	{
		/// <summary>
		/// The zoom of the camera. This can be thought of as the number of spatial units wide the camera's view is.
		/// A higher value signifies a viewport covering a greater area.
		/// </summary>
		public float Zoom
		{
			get => _zoom;
			set
			{
				value = Math.Max(value, Config.MinimumCameraZoom);
				if(value == _zoom)
				{
					return;
				}
				_zoom = value;
			}
		}
		private float _zoom;

		/// <summary> The speed at which the camera zooms. </summary>
		public float ZoomSpeed { get; set; } = 1f;

		/// <summary> The speed at which the camera pans. </summary>
		public float PanSpeed { get; set; } = 1f;

		/// <summary> The speed at which the camera rolls. </summary>
		public float RollSpeed { get; set; } = 100f;

		public static Camera Create(Entity owner)
		{
			Camera c = new Camera(owner);
			owner.World.OnComponentCreated(c);
			return c;
		}

		protected Camera(Entity owner) : base(owner)
		{
			Zoom = Config.DefaultCameraZoom;
			World.Updating += Update;
		}

		public void Update(GameTime gameTime)
		{
			if (World.Keyboard.IsKeyDown(Keys.OemMinus))
			{
				Zoom *= 1 + ZoomSpeed * gameTime.Delta();
			}

			if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
			{
				Zoom *= 1 - ZoomSpeed * gameTime.Delta();
			}

			Vector2 movement = new Vector2(
				(World.Keyboard.IsKeyDown(Keys.D) ? 1 : 0) - (World.Keyboard.IsKeyDown(Keys.A) ? 1 : 0),
				(World.Keyboard.IsKeyDown(Keys.W) ? 1 : 0) - (World.Keyboard.IsKeyDown(Keys.S) ? 1 : 0)
			);

			Owner.LocalTranslation += movement * gameTime.Delta() * PanSpeed * Zoom;

			float rotation = (World.Keyboard.IsKeyDown(Keys.E) ? 1 : 0) - (World.Keyboard.IsKeyDown(Keys.Q) ? 1 : 0);
			Owner.LocalRotation += rotation * RollSpeed * gameTime.Delta();
		}
	}
}
