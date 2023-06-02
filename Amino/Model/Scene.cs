using Amino.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> A base 2D world, into which <see cref="Entity"/> instances can be placed. </summary>
	public class Scene : IGameServiceProvider
    {
		/// <summary> Provides services pertaining to the game's visual systems. May be left as null in the case of a model-only environment. </summary>
		private IViewServiceProvider? _view;

		/// <summary> The game <see cref="GraphicsDevice"/>. </summary>
		private GraphicsDevice? GraphicsDevice => _view?.GraphicsDevice;

		/// <summary>The <see cref="Renderer"/> used to draw visual components.	</summary>
		private Renderer? Renderer { get; init; }

		/// <summary> The game-application service provider. </summary>
		private IGameServiceProvider _game;

		/// <summary> The game <see cref="GameServiceContainer"/>, used to provide generic services. </summary>
		public GameServiceContainer Services => _game.Services;

		/// <summary> The game <see cref="ContentService"/>. </summary>
		public ContentService Content => _game.Content;

		/// <summary> The <see cref="KeyboardManager"/> used to track key states. </summary>
		public KeyboardManager Keyboard => _game.Keyboard;

		/// <summary> The <see cref="Camera"/> used to position the rendering of the game. </summary>
		public Camera Camera { get; private init; }

		/// <summary> Fires each game update. </summary>
		public Action<GameTime> Updating { get; set; }

		public Scene(AminoGame game) : this(game, game)
		{

		}

		public Scene(IGameServiceProvider game) : this(game, null)
		{

		}

		private Scene(IGameServiceProvider game, IViewServiceProvider? view = null) : base()
        {
			_game = game;
			game.Updating += Update;

			Entity cameraEntity = new Entity(this, "Camera");
			Camera = Camera.Create(cameraEntity);

			if (view != null)
			{
				_view = view;
				Renderer = new Renderer(game, view, Camera);
			}
		}

		public T GetService<T>() where T : class => Services.GetService<T>();

		protected void Update(GameTime gameTime)
		{
			Updating?.Invoke(gameTime);
		}

		public void OnComponentCreated(Component newComponent)
		{
			if (newComponent is Sprite asSpriteComponent)
			{
				Renderer?.RegisterSprite(asSpriteComponent);
			}
		}

		public void OnComponentDestroyed(Component destroyedComponent)
		{
			if (destroyedComponent is Sprite asSpriteComponent)
			{
				Renderer?.UnregisterSprite(asSpriteComponent);
			}
		}
	}
}
