using Amino;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eukaryode
{
	/// <summary>
	/// Represents a 2D grid of <see cref="LayerCell"/>s, where each cell may contain an uneven number of layers.
	/// </summary>
	public class LayerGrid : Component
	{
		private LayerCell[,] _cells;

		/// <summary> The horizontal cell-count of the grid. </summary>
		public int Width { get; private init; }
		/// <summary> The vertical cell-count of the grid. </summary>
		public int Height { get; private init; }

		/// <summary> The total number of cells in the grid. </summary>
		public int CellCount { get; private init; }

		public LayerCell this[int index]
		{
			get
			{
				To2DIndex(index, out int x, out int y);
				return _cells[x, y];
			}
			set
			{
				To2DIndex(index, out int x, out int y);
				if (_cells[x, y] == value)
				{
					return;
				}
				_cells[x, y] = value;
			}
		}

		public static LayerGrid Create(Entity owner, int width, int height)
		{
			LayerGrid c = new LayerGrid(owner, width, height);
			owner.World.OnComponentCreated(c);
			return c;
		}

		protected LayerGrid(Entity owner, int width, int height) : base(owner)
		{
			Width = width;
			Height = height;
			CellCount = Width * Height;
			_cells = new LayerCell[width, height];

			for (int c = 0; c < CellCount; c++)
			{
				Entity cellEntity = new Entity(Owner, "LayerCell");
				To2DIndex(c, out int x, out int y);
				Vector2 coord = new Vector2(x, y);
				cellEntity.LocalTranslation = coord;
				this[c] = LayerCell.Create(cellEntity);

				this[c].SurfaceAltitude = MathF.Sin(Vector2.Distance(coord, new Vector2(Width / 2f, (Height / 2f))) * 0.5f) * 50f;
			}
		}

		private void To2DIndex(int index, out int x, out int y)
		{
			y = index % Width;
			x = index / Width;
		}
	}
}
