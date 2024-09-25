using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Amino
{
	/// <summary> Signifies that a class can run the view of a <see cref="Scene"/>. </summary>
	public interface IViewServiceProvider
	{
		public GraphicsDevice GraphicsDevice { get; }

		public GameWindow Window { get; }

		public Vector2Int ViewportDimensions
			=> new Vector2Int(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);

		public Action<GameTime> Drawing { get; set; }
	}
}
