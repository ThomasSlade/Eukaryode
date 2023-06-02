using Amino.Services;
using Autofac.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Amino
{
	/// <summary> A lightweight alternative to <see cref="AminoGame"/> which can be used for unit testing. </summary>
	public class DummyGame : IGameServiceProvider
	{
		public GameServiceContainer Services { get; private set; }
		public new ContentService Content { get; private set; }
		public KeyboardManager Keyboard { get; private set; }

		public Action<GameTime> Updating { get; set; }
		public Action<GameTime> Drawing { get; set; }

		public DummyGame()
		{
			Services = new GameServiceContainer();
			Content = new ContentService(new ContentManager(Services), "Content");
			Keyboard = new KeyboardManager();
		}
	}
}
