using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace Eukaryode
{
	/// <summary>
	/// The type of environment a given layer in the Eukaryode world may represent.
	/// Biolayers are the type of physical habitat present on a cell-layer.
	/// The type of biolayer determines which organisms may occupy that layer, and what types of
	/// layer can be traversed to from that layer.
	/// </summary>
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Biolayer : IComparable<Biolayer>
	{
		/// <summary>
		/// Though there are many biolayer types, all fall into these categories.
		/// </summary>
		public enum Type
		{
			Air,
			OceanSurface,
			WaterColumn,
			Surface,
			Underground
		}

		/// <summary> The type of this biolayer. </summary>
		[JsonProperty]
		public Type BiolayerType { get; private set; }

		/// <summary> Determines the order of the biolayer relative to other biolayers, with 0 signifying surface, and lower numbers being subterranean. </summary>
		[JsonProperty]
		public int LayerIndex { get; private set; }

		/// <summary> If this is a <see cref="Type.WaterColumn"/>, this is the surface altitude at which this layer is enabled. </summary>
		[JsonProperty]
		public float OceanDepth { get; private set; }

		/// <summary> The unique key of this biolayer. </summary>
		[JsonProperty]
		public string Key { get; private init; } = "";

		/// <summary> The key of the sprite used to represent this biolayer. </summary>
		[JsonProperty]
		public string SpriteKey { get; private init; } = "";

		/// <summary> The color used to tint the biolayer sprite. </summary>
		[JsonProperty]
		public Color SpriteColor { get; private init; } = Color.White;

		public Biolayer(string key)
		{
			Key = key;
		}

		public override string ToString() => $"({nameof(Biolayer)}) {Key}";

		public int CompareTo(Biolayer? other) => LayerIndex.CompareTo(other.LayerIndex);
	}
}
