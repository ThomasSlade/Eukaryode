using Amino;
using Microsoft.Xna.Framework;

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

		public LayerCell this[int x, int y]
		{
			get => _cells[x, y];
			set => _cells[x, y] = value;
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
			}
		}

		private void To2DIndex(int index, out int x, out int y)
		{
			x = index % Width;
			y = index / Width;
		}
	}
}
