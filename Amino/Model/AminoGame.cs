﻿using Microsoft.Xna.Framework;
using System;

namespace Amino
{
	/// <summary> A game created with the Amino framework. Provides basic utilities and services which elaborate on the Monogame <see cref="Game"/>. </summary>
	public class AminoGame : Game, IGameServiceProvider, IViewServiceProvider
    {
		/// <summary> The game's <see cref="GraphicsDeviceManager"/>. This is automatically injected into the template of a <see cref="Game"/> class. </summary>
		private GraphicsDeviceManager _graphics;

		/// <summary> The game's <see cref="KeyboardManager"/>. </summary>
		public KeyboardManager Keyboard { get; private init; }

		/// <summary> The colour rendered where nothing else is rendered. </summary>
		protected Color BackgroundColor { get; set; } = Color.CornflowerBlue;

		/// <summary> Fires each game update. </summary>
		public Action<GameTime> Updating { get; set; }
		/// <summary> Fires each rendering update. </summary>
		public Action<GameTime> Drawing { get; set; }
		public new ContentService Content { get; set; }

		public AminoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			Keyboard = new KeyboardManager();
			Services.AddService<KeyboardManager>(Keyboard);
			Content = new ContentService(base.Content, "Content");
			
            IsMouseVisible = true;
        }

		public T GetService<T>() where T : class => Services.GetService<T>();

		protected override void Update(GameTime gameTime)
		{
			Keyboard.Update(gameTime);

			base.Update(gameTime);
			Updating?.Invoke(gameTime);
		}

		protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(BackgroundColor);
			base.Draw(gameTime);
            Drawing?.Invoke(gameTime);
        }
	}
}
