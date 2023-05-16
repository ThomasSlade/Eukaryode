using Amino;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Eukaryode
{
	/// <summary> The application class for Eukaryode. </summary>
	public class Eukaryode : AminoGame
    {
		/// <summary> The main game scene. </summary>
		private Scene _scene;

		private Entity _mobilePineapple;

		public Eukaryode() : base()
        {
			Config.DefaultSprite = "white_square";
        }

        protected override void Initialize()
        {
            base.Initialize();
            _scene = new Scene(this);

			_mobilePineapple = new Entity(_scene);
			new SpriteComponent(_mobilePineapple, "pineapple");
			_mobilePineapple.LocalScale = Vector2.One * 4f;
			_mobilePineapple.LocalTranslation += Vector2.UnitY * 5f;

			Entity otherPineapple = new Entity(_scene);
			new SpriteComponent(otherPineapple, "pineapple");
			otherPineapple.LocalTranslation += Vector2.UnitY * 5f;
		}

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.IsKeyDown(Keys.Escape))
                Exit();

			// Move the pineapple.
			Vector2 movement = new Vector2(
				(Keyboard.IsKeyDown(Keys.L) ? 1 : 0) - (Keyboard.IsKeyDown(Keys.J) ? 1 : 0),
				(Keyboard.IsKeyDown(Keys.I) ? 1 : 0) - (Keyboard.IsKeyDown(Keys.K) ? 1 : 0)
			);

			_mobilePineapple.LocalTranslation += movement * gameTime.Delta();

			float rotation = (Keyboard.IsKeyDown(Keys.O) ? 100 : 0) - (Keyboard.IsKeyDown(Keys.U) ? 100 : 0);
			_mobilePineapple.LocalRotation += rotation * gameTime.Delta();

			Vector2 scale = new Vector2(
				(Keyboard.IsKeyDown(Keys.OemOpenBrackets) ? 1 : 0) - (Keyboard.IsKeyDown(Keys.OemCloseBrackets) ? 1 : 0),
				(Keyboard.IsKeyDown(Keys.OemComma) ? 1 : 0) - (Keyboard.IsKeyDown(Keys.OemPeriod) ? 1 : 0)
			);

			_mobilePineapple.LocalScale += scale * gameTime.Delta();

			base.Update(gameTime);
        }
    }
}