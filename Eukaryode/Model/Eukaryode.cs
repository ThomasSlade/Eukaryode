using Amino;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Eukaryode
{
	/// <summary> The application class for Eukaryode. </summary>
	public class Eukaryode : AminoGame
    {
		/// <summary> The main game scene. </summary>
		private Scene _scene;

		public Eukaryode() : base()
        {
			Config.DefaultSprite = "white_square";

			Services.AddService<BiolayerService>(new BiolayerService(this));
        }

        protected override void Initialize()
        {
            base.Initialize();

            _scene = new Scene(this);

			Sprite checkers = Sprite.Create(new Entity(_scene), "coloured_checkers");
			checkers.OffsetType = AnchorType.Centre;
			checkers.PixelsPerUnit = 1f;

			Entity gridEntity = new Entity(_scene);
			LayerGrid.Create(gridEntity, 20, 20);
		}

        protected override void Update(GameTime gameTime)
        {
			base.Update(gameTime);

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }
    }
}