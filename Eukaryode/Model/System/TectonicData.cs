using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eukaryode
{
	/// <summary> Defines the initial state of a world's map and its progression over time. </summary>
	public class TectonicData
	{
		private Texture2D _initialHeightmap;

		public TectonicData(Texture2D initialHeightmap)
		{
			_initialHeightmap = initialHeightmap;
		}
	}
}
