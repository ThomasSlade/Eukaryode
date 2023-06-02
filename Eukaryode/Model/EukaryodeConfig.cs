namespace Eukaryode
{
	/// <summary> Config variables for Eukaryode. </summary>
	public static class EukaryodeConfig
	{
		/// <summary> The pixels per unit used for terrain tiles. </summary>
		public const float TerrainPixelsPerUnit = 64f;

		/// <summary>
		/// The lowest (for below sea level) or highest altitude that colour lerping reaches for a terrain tile.
		/// Is negated for below sea level.
		/// </summary>
		public const float MaxColourAltitude = 100f;
	}
}
