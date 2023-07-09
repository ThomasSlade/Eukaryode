using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eukaryode.Tectonics
{
	/// <summary> Data relating to the setup of a <see cref="TectonicWorld"/>. </summary>
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class TectonicData
	{
		[JsonProperty]
		public string Test { get; set; } = "";

		public IReadOnlyList<TectonicPlateData> Plates => _plates;
		[JsonProperty("Plates")]
		private List<TectonicPlateData> _plates { get; set; }

		/// <summary> All vertex colours mapped to the plates they occur in. </summary>
		private Dictionary<Color, TectonicPlateData> _vertsToPlates = new Dictionary<Color, TectonicPlateData>();

		public TectonicData(IList<TectonicPlateData> plates)
		{
			_plates = plates.ToList();
			foreach (TectonicPlateData plate in Plates)
			{
				foreach (Color vert in plate.Vertices)
				{
					if(!_vertsToPlates.TryAdd(vert, plate))
					{
						throw new InvalidDataException($"Plate Data '{_vertsToPlates[vert]}' and '{plate}' both contain vertex '{vert}'");
					}
				}
			}
		}

		/// <summary> Attempt to get the plate that owns the vertex of this colour. </summary>
		public bool TryGetPlate(Color vert, out TectonicPlateData plate)
			=> _vertsToPlates.TryGetValue(vert, out plate);
	}
}
