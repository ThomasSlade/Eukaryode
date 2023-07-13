using Microsoft.Xna.Framework;
using System;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Amino
{
	/// <summary> A game created with the Amino framework. Provides basic utilities and services which elaborate on the Monogame <see cref="Game"/>. </summary>
	public class AminoGame : Game, IGameServiceProvider, IViewServiceProvider
    {
		/// <summary> The game's <see cref="GraphicsDeviceManager"/>. This is automatically injected into the template of a <see cref="Game"/> class. </summary>
		private GraphicsDeviceManager _graphics;

		/// <summary> The immediate GUI renderer used to render Debug UI. </summary>
		private ImGuiRenderer _imGuiRenderer;

		/// <summary> The game's <see cref="KeyboardManager"/>. </summary>
		public KeyboardManager Keyboard { get; private init; }

		/// <summary> The colour rendered where nothing else is rendered. </summary>
		protected Color BackgroundColor { get; set; } = Color.CornflowerBlue;

		/// <summary> Fires each game update. </summary>
		public Action<GameTime> Updating { get; set; }
		/// <summary> Fires each rendering update. </summary>
		public Action<GameTime> Drawing { get; set; }
		/// <summary> Fires when constructing the ImGui interface. </summary>
		public Action<GameTime> ImGuiUpdating { get; set; }
		public new ContentService Content { get; set; }

		/// <summary> When true, imGui will be rendered and imgui updates will run. </summary>
		private bool _imguiEnabled = false;

		public AminoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1600;
			_graphics.PreferredBackBufferHeight = 900;
			_imGuiRenderer = new ImGuiRenderer(this, this);
			
			Keyboard = new KeyboardManager();
			Services.AddService<KeyboardManager>(Keyboard);
			Content = new ContentService(base.Content, "Content");
			
            IsMouseVisible = true;
        }

		protected override void Initialize()
		{
			base.Initialize();
			_imGuiRenderer.RebuildFontAtlas();
		}

		public T GetService<T>() where T : class => Services.GetService<T>();

		protected override void Update(GameTime gameTime)
		{
			Keyboard.Update(gameTime);

			base.Update(gameTime);
			Updating?.Invoke(gameTime);

			if(Keyboard.IsKeyPressed(Config.ImGuiKey))
			{
				_imguiEnabled = !_imguiEnabled;
			}
		}

		protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(BackgroundColor);
			base.Draw(gameTime);
            Drawing?.Invoke(gameTime);

			if(_imguiEnabled)
			{
				_imGuiRenderer.BeforeImGui(gameTime);
				ImGui.Begin(GetType().Name);
				ImGuiUpdating?.Invoke(gameTime);
				ImGui.End();
				_imGuiRenderer.AfterImGui();
			}
		}
	}
}
