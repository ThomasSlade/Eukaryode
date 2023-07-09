using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Eukaryode.Tectonics
{
	/// <summary> Data relating to an individual plate on a <see cref="TectonicWorld"/>. </summary>
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class TectonicPlateData : IComparable<TectonicPlateData>
	{
		/// <summary> The name of the plate. </summary>
		[JsonProperty]
		public string Name { get; private set; } = "";

		[JsonProperty]
		public Color KeyColor { get; private set; }

		/// <summary> Any colours on the tectonic map which define the corners of the plate and how they move over time. </summary>
		public IReadOnlySet<Color> Vertices => _vertices;
		[JsonProperty("Vertices")]
		public HashSet<Color> _vertices { get; private set; } = new HashSet<Color>();

		public int CompareTo(TectonicPlateData? other) => Name.CompareTo(other?.Name);

		public override string ToString() => Name;
	}
}
