using Microsoft.Xna.Framework;
using System;

namespace Amino
{
	/// <summary> Misc utilities for Amino. </summary>
	public class AminoUtils
	{
		private static Random random = new Random();

		/// <summary> Get a color with random R, G, and B values. </summary>
		public static Color RandomColor()
			=> new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
	}
}
