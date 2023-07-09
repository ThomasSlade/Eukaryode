using Microsoft.Xna.Framework;

namespace Amino
{
	/// <summary>
	/// A service with lifecycle methods, such as Update.
	/// </summary>
	public abstract class LifecycleService
	{
		public LifecycleService(AminoGame game)
		{
			game.Updating += Update;
		}

		protected virtual void Update(GameTime gameTime)
		{

		}
	}
}
