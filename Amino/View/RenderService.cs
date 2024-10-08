﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> Renders visual components. </summary>
	public class RenderService
	{
		/// <summary> The service provider for model services. </summary>
		private IGameServiceProvider _game;

		/// <summary> The service provider for the renderer. </summary>
		private IViewServiceProvider _view;

		/// <summary> The <see cref="GraphicsDevice"/> of the renderer. </summary>
		public GraphicsDevice GraphicsDevice => _view.GraphicsDevice;
		/// <summary> The <see cref="ContentService"/> of the renderer. </summary>
		public ContentService Content => _game.Content;

		/// <summary> All <see cref="SpriteRenderer"/>s. There should be one for each <see cref="Sprite"/>. </summary>
		private Dictionary<Sprite, SpriteRenderer> _spriteRenderers = new Dictionary<Sprite, SpriteRenderer>();

		/// <summary> The renderer used to support drawing debug geometry. </summary>
		public DebugRenderer Debug => _debug;
		private DebugRenderer _debug;

		/// <summary> The main sprite batch. </summary>
		private SpriteBatch _spriteBatch;

		/// <summary> The camera being rendered with. </summary>
		public Camera _camera;

		/// <summary>
		/// The number of units-wide each pixel is, given the camera's zoom.
		/// If the zoom signified that 25 units should fit on screen, and the screen is 100 pixels, this will be (0.25, 0.25).
		/// </summary>
		private Matrix3x3 _cameraScaleMatrix;
		/// <summary>
		/// The matrix used to convert the camera's coordiante system into the screen's pixel coordinate system.
		/// This is used to make an increasing Y coordinate signify moving upwards on screen, rather than the default downwards.
		/// </summary>
		public Matrix3x3 _cameraScreenMatrix;
		/// <summary>
		/// In pixel-space, the position by which the camera should be offset relative to the position of its transform.
		/// This is typically half of the screen width and screen height, negated, to bring the camera's transform into the centre of the screen.
		/// </summary>
		private Matrix3x3 _cameraOffsetMatrix;


		/// <summary>
		/// The matrix by which to transform entities relative to the camera when rendering.
		/// This is equal to the camera's world transform, scaled by <see cref="_cameraScaleMatrix"/>,
		/// offset by <see cref="_cameraOffsetMatrix"/>,
		/// </summary>
		/// <example>
		/// To obtain an object's position and rotation in screenspace:
		/// <code>
		/// Matrix3x3 renderTransform = camera.RenderTransform * Object.Transform;
		/// _spriteBatch.Draw(_texture, renderTransform.Translation, Color.White);
		/// </code>
		/// </example>
		private Matrix3x3 CameraRenderTransform { get; set; }

		public RenderService(IGameServiceProvider game, IViewServiceProvider view, Camera camera)
		{
			_game = game;
			_view = view;
			_camera = camera;

			_cameraOffsetMatrix = new Matrix3x3();
			_cameraOffsetMatrix.Translation = new Vector2(
				-view.ViewportDimensions.X * 0.5f,
				-view.ViewportDimensions.Y * 0.5f
			);

			_cameraScreenMatrix = new Matrix3x3();
			_cameraScreenMatrix.Scale *= new Vector2(1f, -1f);

			_debug = new DebugRenderer(this);
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			view.Drawing += Draw;
		}

		protected void Draw(GameTime gameTime)
		{
			// Say the screen is 500 pixels wide, and we want to show 250 units. We want a scale matrix of (0.5, 0.5),
			// meaning each pixel contains half a unit..
			float unitsPerPixel = _camera.Zoom / _view.ViewportDimensions.X;
			_cameraScaleMatrix = Matrix3x3.ScaleMatrix(new Vector2(unitsPerPixel, unitsPerPixel));

			CameraRenderTransform = (_camera.Owner.Transform * _cameraScaleMatrix * _cameraScreenMatrix * _cameraOffsetMatrix).GetInverse();

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			foreach (SpriteRenderer renderer in _spriteRenderers.Values)
			{
				renderer.Draw(gameTime, ref _spriteBatch, CameraRenderTransform);
			}

			_debug.Draw(gameTime, ref _spriteBatch, CameraRenderTransform);

			_spriteBatch.End();
		}

		/// <summary> Add a sprite to be rendered. </summary>
		public void RegisterSprite(Sprite sprite)
		{
			if (!_spriteRenderers.TryAdd(sprite, new SpriteRenderer(this, sprite)))
			{
				throw new InvalidOperationException($"Sprite '{sprite}' tried to give itself a {nameof(SpriteRenderer)} but one already exists for this sprite.");
			}
		}

		/// <summary> Remove a sprite which should no longer be rendered. </summary>
		public void UnregisterSprite(Sprite sprite)
		{
			if (!_spriteRenderers.Remove(sprite))
			{
				throw new InvalidOperationException($"Sprite '{sprite}' tried to remove its {nameof(SpriteRenderer)} but no renderer was present for this sprite.");
			}
		}
	}
}
