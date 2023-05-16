using Microsoft.Xna.Framework;

namespace Amino
{
    /// <summary>
    /// Extensions relating to math classes.
    /// </summary>
    public static class MathExtensions
    {
		public static Vector2 Perpendicular(this Vector2 vector2, bool clockwise)
		{
			return clockwise ? new Vector2(vector2.Y, -vector2.X) : new Vector2(-vector2.Y, vector2.X);
		}

		/// <summary>
		/// Shorthand for the elapsed seconds of game time.
		/// </summary>
		public static float Delta(this GameTime gameTime) => (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}
