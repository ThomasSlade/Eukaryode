using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> Extends the functionality of the <see cref="Keyboard"/> class, providing key-down and key-up tracking. </summary>
	public class KeyboardManager
	{
		private enum KeyState
		{
			/// <summary> The key is not being held. </summary>
			Up,
			/// <summary> The key has been released this update. </summary>
			Released,
			/// <summary> The key is being held. </summary>
			Down,
			/// <summary> The key has been pressed this update. </summary>
			Pressed
		}

		/// <summary> The state of each key. </summary>
		private Dictionary<Keys, KeyState> _keyStates = new Dictionary<Keys, KeyState>();

		public KeyboardManager()
		{
			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				// If keys are held when the application has just started, don't interpret it as a 'pressed this frame'. Instead, just note that the key is pressed.
				_keyStates.Add(key, Keyboard.GetState().IsKeyDown(key) ? KeyState.Down : KeyState.Up);
			}
		}

		public void Update(GameTime gameTime)
		{
			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				bool isDown = Keyboard.GetState().IsKeyDown(key);

				// Where a key has been pressed, or released, for more than one frame,
				// adjust its 'pressed / released this frame' state to just a 'pressed / released' state.
				if (isDown)
				{
					switch (_keyStates[key])
					{
						case KeyState.Up:
						case KeyState.Released:
							_keyStates[key] = KeyState.Pressed;
							break;
						case KeyState.Pressed:
							_keyStates[key] = KeyState.Down;
							break;
					}
				}
				else
				{
					switch (_keyStates[key])
					{
						case KeyState.Released:
							_keyStates[key] = KeyState.Up;
							break;
						case KeyState.Down:
						case KeyState.Pressed:
							_keyStates[key] = KeyState.Released;
							break;
					}
				}
			}
		}

		/// <summary> Is the key being held. </summary>
		public bool IsKeyDown(Keys key) => _keyStates[key] == KeyState.Down || _keyStates[key] == KeyState.Pressed;

		/// <summary> Has the key been pressed this update. </summary>
		public bool IsKeyPressed(Keys key) => _keyStates[key] == KeyState.Pressed;

		/// <summary> Is the key not being held. </summary>
		public bool IsKeyUp(Keys key) => _keyStates[key] == KeyState.Up || _keyStates[key] == KeyState.Released;

		/// <summary> Has the key been released this update. </summary>
		public bool IsKeyReleased(Keys key) => _keyStates[key] == KeyState.Released;
	}
}
