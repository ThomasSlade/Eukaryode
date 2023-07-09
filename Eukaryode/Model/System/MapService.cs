using Amino;
using Eukaryode.Tectonics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Eukaryode
{
	/// <summary> Services relating to the game map. </summary>
	public class MapService
	{
		/// <summary> The location at which maps are kept in content. </summary>
		private const string MapDirectory = "Maps";

		private ContentService _content;

		public MapService(IGameServiceProvider serviceProvider)
		{
			_content = serviceProvider.Content;
		}

		public void LoadMap(string mapFolderName, Entity tectonicWorldOwner, out TectonicWorld tectonicWorld)
		{
			TectonicData tectonicData = _content.Load<TectonicData>(Path.Combine(MapDirectory, mapFolderName, "tectonic_data"));
			List<TectonicSnapshot> tectonicSnapshots = new List<TectonicSnapshot>();

			int tectonicIndex = 0;
			while (true)
			{
				string tectonicFilename = "tectonic_" + tectonicIndex;
				string heightmapFilename = "heightmap_" + tectonicIndex;
				if (!_content.TryLoad<Texture2D>(Path.Combine(MapDirectory, mapFolderName, tectonicFilename),
					out Texture2D tectonicMap))
				{
					break;
				}

				if (!_content.TryLoad<Texture2D>(Path.Combine(MapDirectory, mapFolderName, heightmapFilename),
					out Texture2D heightMap))
				{
					throw new InvalidDataException($"No heightmap with the same index as '{tectonicFilename}' was present.");
				}

				tectonicSnapshots.Add(new TectonicSnapshot(tectonicFilename, tectonicData, tectonicMap, heightMap, tectonicIndex * 10f));
				tectonicIndex++;
			}

			tectonicWorld = new TectonicWorld(tectonicWorldOwner, tectonicData, tectonicSnapshots);
		}
	}
}
