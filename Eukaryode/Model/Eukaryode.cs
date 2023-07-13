using Amino;
using Eukaryode.Tectonics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Eukaryode
{
	/// <summary> The application class for Eukaryode. </summary>
	public class Eukaryode : AminoGame
    {
		/// <summary> The main game scene. </summary>
		private Scene _scene;

		TectonicWorld _tectonicWorld;

		Entity testBerry;

		public Eukaryode() : base()
        {
			Config.DefaultSprite = "white_square";

			Services.AddService(new GeoTimeService(this));
			Services.AddService(new BiolayerService(this));
			Services.AddService(new MapService(this));
        }

        protected override void Initialize()
		{
			base.Initialize();

			_scene = new Scene(this);

			Sprite checkers = Sprite.Create(new Entity(_scene), "coloured_checkers");
			checkers.OffsetType = AnchorType.Centre;
			checkers.PixelsPerUnit = 1f;

			Entity tectonicWorldEntity = new Entity(_scene, "TectonicWorld");
			Services.GetService<MapService>().LoadMap("Earth", tectonicWorldEntity, out _tectonicWorld);

			testBerry = new Entity(_scene);
			Sprite.Create(testBerry, "strawberry");
		}

        protected override void Update(GameTime gameTime)
        {
			base.Update(gameTime);

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.IsKeyDown(Keys.Escape))
                Exit();

			if(Keyboard.IsKeyPressed(Keys.P))
			{
				_tectonicWorld.TectonicTick(0.1f);
			}

			float y = Keyboard.IsKeyDown(Keys.I) ? 1f : 0f;
			y += Keyboard.IsKeyDown(Keys.K) ? -1f : 0f;
			float x = Keyboard.IsKeyDown(Keys.L) ? 1f : 0f;
			x += Keyboard.IsKeyDown(Keys.J) ? -1f : 0f;

			testBerry.LocalTranslation += new Vector2(x, y) * gameTime.Delta();
		}
	}
}