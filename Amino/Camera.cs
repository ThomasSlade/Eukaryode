using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Amino
{
	/// <summary> A camera used to position the rendering viewport. </summary>
	public class Camera : Component
	{
		/// <summary>
		/// The transform used to convert worldspace into screenspace.
		/// This is equal to the camera's transform, multiplied by the screen transform (used to flip the image so that downwards on the screen is positive-Y),
		/// and then inverted.
		/// </summary>
		/// <example>
		/// To obtain an object's position and rotation in screenspace:
		/// <code>
		/// Matrix3x3 renderTransform = camera.RenderTransform * Object.Transform;
		/// _spriteBatch.Draw(_texture, renderTransform.Translation, Color.White);
		/// </code>
		/// </example>
		public Matrix3x3 RenderTransform => _renderTransform;
		private Matrix3x3 _renderTransform;

		/// <summary> The transform used to convert from world-space to screen-space, given the camera's rotation and position. </summary>
		private Matrix3x3 _screenTransform;
		/// <summary> The transform used to zoom the viewport. </summary>
		private Matrix3x3 _scaleTransform;

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

				// Say the screen is 500 pixels wide, and we want to show 250 units. We want a scale matrix of (0.5, 0.5).
				float unitDisplayWidth = value / World.GraphicsDevice.Viewport.Bounds.Width;
				_scaleTransform = Matrix3x3.ScaleMatrix(new Vector2(unitDisplayWidth, unitDisplayWidth));
				OnTransformChanged(this, EventArgs.Empty);
			}
		}
		private float _zoom;

		/// <summary> The speed at which the camera zooms. </summary>
		public float ZoomSpeed { get; set; } = 1f;

		/// <summary> The speed at which the camera pans. </summary>
		public float PanSpeed { get; set; } = 1f;

		/// <summary> The speed at which the camera rolls. </summary>
		public float RollSpeed { get; set; } = 100f;

		public Camera(Entity owner) : base(owner)
		{
			_screenTransform = new Matrix3x3();
			_screenTransform.Translation = new Vector2(
				-World.GraphicsDevice.Viewport.Bounds.Width * 0.5f,
				World.GraphicsDevice.Viewport.Bounds.Height * 0.5f
			);
			_screenTransform.Scale *= new Vector2(1f, -1f);
			Zoom = Config.DefaultCameraZoom;

			World.Updating += Update;
			owner.TransformChanged += OnTransformChanged;
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

		private void OnTransformChanged(object? sender, EventArgs e)
		{
			// When the camera moves, recalculate the render transform.
			_renderTransform = (Owner.Transform * _scaleTransform * _screenTransform).GetInverse();
		}
	}
}
