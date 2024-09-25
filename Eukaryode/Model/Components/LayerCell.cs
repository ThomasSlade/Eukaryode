using Amino;
using System;
using System.Collections.Generic;

namespace Eukaryode
{
	/// <summary>
	/// Represents a cell containing layers. The number of layers is variable, and may not be equal to other cells in the grid.
	/// </summary>
	public class LayerCell : Component
	{
		private BiolayerService _biolayerService;
		private SortedDictionary<Biolayer, Layer> _layers = new SortedDictionary<Biolayer, Layer>();

		/// <summary> The altitude the surface of this layer sits at. This can determine if the terrain is under water. </summary>
		public float SurfaceAltitude
		{
			get => _surfaceAltitude;
			set
			{
				if (value == _surfaceAltitude)
				{
					return;
				}
				_surfaceAltitude = value;
				SurfaceAltitudeChanged?.Invoke(this, EventArgs.Empty);
				Submerged = value < 0f;
			}
		}
		private float _surfaceAltitude = 0f;

		/// <summary> Whether or not this layer cell sits below cea-level. If true, the cell will have ocean layers on top of its surface layer. </summary>
		public bool Submerged
		{
			get => _submerged;
			private set
			{
				if (value == _submerged)
				{
					return;
				}
				if (value)
				{
					Submerge();
				}
				else
				{
					Unsubmerge();
				}
				_submerged = value;
			}
		}
		private bool _submerged = false;

		/// <summary> Fires when <see cref="SurfaceAltitude"/> changes. </summary>
		public EventHandler SurfaceAltitudeChanged;

		public static LayerCell Create(Entity owner, float surfaceAltitude = 0f)
		{
			LayerCell l = new LayerCell(owner);
			l.World.OnComponentCreated(l);
			return l;
		}

		protected LayerCell(Entity owner) : base(owner)
		{
			_biolayerService = World.GetService<BiolayerService>();
			AddLayer(_biolayerService.Underground);
			AddLayer(_biolayerService.Surface);
			AddLayer(_biolayerService.Air);
		}

		private bool TryAddLayer(Biolayer biolayer)
		{
			if (_layers.ContainsKey(biolayer))
			{
				return false;
			}

			Entity layerEntity = new Entity(Owner, $"Layer ({biolayer.Key})");
			Layer newLayer = Layer.Create(layerEntity, this, biolayer);
			_layers.Add(biolayer, newLayer);
			return true;
		}

		private bool TryAddLayer(string biolayerKey) => TryAddLayer(World.Content.Load<Biolayer>(biolayerKey));

		private void AddLayer(Biolayer biolayer)
		{
			if (!TryAddLayer(biolayer))
			{
				throw new InvalidOperationException($"Layer with biolayer type '{biolayer}' is already present in cell '{this}'.");
			}
		}

		private void AddLayer(string biolayerKey) => AddLayer(World.Content.Load<Biolayer>(biolayerKey));

		private bool RemoveLayer(Biolayer biolayer)
		{
			if (!_layers.ContainsKey(biolayer))
			{
				return false;
			}
			_layers[biolayer].Owner.Destroy();
			_layers.Remove(biolayer);
			return true;
		}

		private void Submerge()
		{
			AddLayer(_biolayerService.OceanSurface);
			foreach (Biolayer waterColumn in _biolayerService.AllWaterColumns)
			{
				if (waterColumn.OceanDepth > SurfaceAltitude)
				{
					break;
				}
				AddLayer(waterColumn);
			}
		}

		private void Unsubmerge()
		{
			RemoveLayer(_biolayerService.OceanSurface);
			foreach (Biolayer waterColumn in _biolayerService.AllWaterColumns)
			{
				RemoveLayer(waterColumn);
			}
		}
	}
}
