using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
