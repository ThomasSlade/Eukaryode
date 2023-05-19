using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eukaryode
{
	/// <summary> Represents a 3D grid and the contents of that grid's cells. </summary>
	public class Grid3DComponent : IEnumerable<GridCell>
	{
		private GridCell[,,] _cells;

		/// <summary> The horizontal cell-count of the grid. </summary>
		public int Width { get; private init; }
		/// <summary> The vertical cell-count of the grid. </summary>
		public int Height { get; private init; }
		/// <summary> The screenward cell-count of the grid. </summary>
		public int Depth { get; private init; }
		/// <summary> The total number of cells in the 3D grid. </summary>
		public int CellCount { get; private init; }
		/// <summary> The number of cells in a single layer of the 3D grid, equal to Width * Height. </summary>
		public int CellLayerCount { get; private init; }

		public GridCell this[int index]
		{
			get
			{
				To3DIndex(index, out int x, out int y, out int z);
				return _cells[x, y, z];
			}
			set
			{
				To3DIndex(index, out int x, out int y, out int z);
				if (_cells[x, y, z] == value)
				{
					return;
				}
				_cells[x, y, z] = value;
			}
		}

		public Grid3DComponent(int width, int height, int depth)
		{
			Width = width;
			Height = height;
			Depth = depth;
			CellCount = Width * Height * Depth;
			CellLayerCount = Width * Height;
			_cells = new GridCell[width, height, depth];
			for (int c = 0; c < CellCount; c++)
			{
				this[c] = new GridCell();
			}
;		}

		private void To3DIndex(int index, out int x, out int y, out int z)
		{
			z = index % CellLayerCount;
			index -= z;
			y = index % Width;
			x = index - y;
		}

		public IEnumerator<GridCell> GetEnumerator()
		{
			int index = 0;
			while(index < CellCount)
			{
				yield return this[index];
				index++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
