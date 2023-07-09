using Amino;
using Microsoft.Xna.Framework;

namespace Eukaryode
{
	/// <summary> The in-game time system, which tracks time in millions of years since the game start date. </summary>
	public class GeoTimeService : LifecycleService
	{
		/// <summary> How much time has passed, in millions of years, since the start of the game. </summary>
		public float MilYears { get; private set; } = 0f;

		public GeoTimeService(AminoGame game) : base(game)
		{

		}

		protected override void Update(GameTime gameTime)
		{
			MilYears += gameTime.Delta() * EukaryodeConfig.MilYearsPerSecond;
		}
	}
}
