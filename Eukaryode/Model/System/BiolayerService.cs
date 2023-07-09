using Amino;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Eukaryode
{
	/// <summary> Services relating to <see cref="Biolayer"/>s. </summary>
	public class BiolayerService
	{
		/// <summary> The content directory at which biolayers are defined. </summary>
		private const string ContentDirectory = "Data/Biolayers";
		/// <summary> All biolayers that have been loaded. </summary>
		private Biolayer[] _allBiolayers = new Biolayer[0];
		/// <summary> All biolayers whose type is <see cref="Biolayer.Type.WaterColumn"/>. </summary>
		public IReadOnlyCollection<Biolayer> AllWaterColumns;
		private Biolayer[] _allWaterColumns = new Biolayer[0];

		public Biolayer Air => _air;
		private Biolayer _air;
		public Biolayer OceanSurface => _oceanSurface;
		private Biolayer _oceanSurface;
		public Biolayer Surface => _surface;
		private Biolayer _surface;
		public Biolayer Underground => _underground;
		private Biolayer _underground;

		public BiolayerService(IGameServiceProvider services)
		{
			_allBiolayers = services.Content.LoadAllResources<Biolayer>(ContentDirectory).ToArray();
			HashSet<int> usedIndices = new HashSet<int>();
			foreach(Biolayer biolayer in _allBiolayers)
			{
				if(!usedIndices.Add(biolayer.LayerIndex))
				{
					throw new InvalidDataException($"Biolayer '{biolayer}' has non-unique {nameof(Biolayer.LayerIndex)} '{biolayer.LayerIndex}'.");
				}
			}

			foreach (Biolayer biolayer in _allBiolayers)
			{
				switch(biolayer.BiolayerType)
				{
					case Biolayer.Type.Air: SetConstantLayer(ref _air, biolayer); break;
					case Biolayer.Type.OceanSurface: SetConstantLayer(ref _oceanSurface, biolayer); break;
					case Biolayer.Type.Surface: SetConstantLayer(ref _surface, biolayer); break;
					case Biolayer.Type.Underground: SetConstantLayer(ref _underground, biolayer); break;
				}
			}

			_allWaterColumns = _allBiolayers.Where(b => b.BiolayerType == Biolayer.Type.WaterColumn).ToArray();
			Array.Sort(_allWaterColumns);

			float highestDepth = float.MinValue;
			Biolayer prev = null;
			foreach (Biolayer waterColumn in _allWaterColumns)
			{
				if(waterColumn.OceanDepth <= highestDepth)
				{
					throw new InvalidDataException($"Biolayer '{waterColumn}'s {nameof(Biolayer.OceanDepth)} property was not higher than its predecessor in the layer order: '{prev}'.");
				}
				prev = waterColumn;
			}

			AllWaterColumns = new ReadOnlyCollection<Biolayer>(_allWaterColumns);
		}

		private void SetConstantLayer(ref Biolayer layer, Biolayer value)
		{
			Debug.Assert(layer == null, $"Layers '{layer}' and '{value}' have the same type but only one biolayer of type '{value.BiolayerType}'.");
			layer = value;
		}
	}
}
