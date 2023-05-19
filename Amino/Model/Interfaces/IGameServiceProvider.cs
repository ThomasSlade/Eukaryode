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
	/// <summary> Signifies that a class can provide the services required to run a <see cref="Scene"/>. </summary>
	public interface IGameServiceProvider
	{
		public KeyboardManager Keyboard { get; }

		public Action<GameTime> Updating { get; set; }
	}
}
