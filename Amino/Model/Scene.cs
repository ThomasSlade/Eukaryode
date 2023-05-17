using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Amino
{
	/// <summary> A base 2D world, into which <see cref="Entity"/> instances can be placed. </summary>
	public class Scene
    {
        /// <summary> The game <see cref="ContentManager"/>. </summary>
        public ContentManager Content { get; init; }

        /// <summary> The game <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/>. </summary>
        public GraphicsDevice GraphicsDevice { get; init; }

		/// <summary> The <see cref="KeyboardManager"/> used to track key states. </summary>
		public KeyboardManager Keyboard { get; init; }

		/// <summary> The <see cref="Camera"/> used to position the rendering of the game. </summary>
		public Camera Camera { get; private init; }

		/// <summary> Fires each game update. </summary>
		public Action<GameTime> Updating;
		/// <summary> Fires each rendering update. </summary>
		public Action<GameTime> Drawing;

		public Scene(AminoGame game) : base()
        {
            Content = game.Content;
            GraphicsDevice = game.GraphicsDevice;
			Keyboard = game.Keyboard;

			Entity cameraEntity = new Entity(this, "Camera");
			Camera = new Camera(cameraEntity);

			game.Updating += Update;
			game.Drawing += Draw;
        }

		protected void Update(GameTime gameTime)
		{
			Updating?.Invoke(gameTime);
		}

        protected void Draw(GameTime gameTime)
        {
			Drawing?.Invoke(gameTime);
        }
    }
}
