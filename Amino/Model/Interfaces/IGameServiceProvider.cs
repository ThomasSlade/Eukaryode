using Microsoft.Xna.Framework;
using System;

namespace Amino
{
	/// <summary> Signifies that a class can provide the services required to run a <see cref="Scene"/>. </summary>
	public interface IGameServiceProvider
	{
		/// <summary> Any services not explicitly defined in the <see cref="IGameServiceProvider"/>. </summary>
		public GameServiceContainer Services { get; }
		public ContentService Content { get; }
		public KeyboardManager Keyboard { get; }

		public Action<GameTime> Updating { get; set; }
	}
}
