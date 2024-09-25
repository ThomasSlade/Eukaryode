using Amino;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eukaryode.Tectonics
{
	/// <summary> A state of the world's surface as defined by image data, and between which the game world can be interpolated. </summary>
	public class TectonicSnapshot
	{
		private string Name { get; set; }
		private TectonicData _data;
		public Texture2D TectonicMap { get; private init; }
		public Texture2D Heightmap { get; private init; }

		private float[,] _altitudes;

		public int Width => TectonicMap.Width;
		public int Height => TectonicMap.Height;

		public IReadOnlyList<TectonicPlateSnapshot> Plates => _plates;
		private List<TectonicPlateSnapshot> _plates;

		/// <summary> Maps all plates, by their index, to the pixels constituting them. </summary>
		private int[,] _plateMappings;

		/// <summary> The paleological time, in millions of years, at which this snapshot sits, with 0 being the start of the game. </summary>
		public float GeologicalTime { get; private init; } = 0f;

		public TectonicSnapshot(string name, TectonicData data, Texture2D tectonicMap, Texture2D heightMap, float geologicalTime)
		{
			Name = name;
			_data = data;

			if (tectonicMap.Width != heightMap.Width || tectonicMap.Height != heightMap.Height)
			{
				throw new ArgumentException($"Cannot create a snapshot using a tectonic map and a height map of differing dimensions ({tectonicMap.Width}x{tectonicMap.Height} vs. {heightMap.Width}x{heightMap.Height}).");
			}

			TectonicMap = tectonicMap;
			Heightmap = heightMap;
			_altitudes = new float[Width, Height];

			GeologicalTime = geologicalTime;

			ReadPlates();
		}

		private void ReadPlates()
		{
			Color[] tectonicPixels = new Color[TectonicMap.Width * TectonicMap.Height];
			TectonicMap.GetData(tectonicPixels);
			Color[] heightPixels = new Color[TectonicMap.Width * TectonicMap.Height];
			Heightmap.GetData(heightPixels);

			List<TectonicPlateSnapshot> plates = new List<TectonicPlateSnapshot>(_data.Plates.Count);

			// Map all colours to the locations they appear on.
			Dictionary<Color, HashSet<Vector2Int>> _pixelsToCoords = new Dictionary<Color, HashSet<Vector2Int>>();
			for (int p = 0; p < tectonicPixels.Length; p++)
			{
				Color pixel = tectonicPixels[p];
				_pixelsToCoords.TryAdd(pixel, new HashSet<Vector2Int>(1));
				Vector2Int imageCoord = MathUtils.UnflattenIndex(p, TectonicMap.Width, TectonicMap.Height);

				// Convert from image space (origin top-left) to grid space (origin bottom-left).
				Vector2Int gridCoord = imageCoord;
				gridCoord.Y = -gridCoord.Y + (Height - 1);
				_pixelsToCoords[pixel].Add(gridCoord);

				_altitudes[gridCoord.X, gridCoord.Y] = heightPixels[p].R / 255f * (EukaryodeConfig.MaxColourAltitude * 2) - EukaryodeConfig.MaxColourAltitude;
			}

			foreach (TectonicPlateData plateData in _data.Plates)
			{
				List<PlateVertex> vertices = new List<PlateVertex>(plateData.Vertices.Count);
				foreach (Color vertCol in plateData.Vertices)
				{
					if (!_pixelsToCoords.ContainsKey(vertCol))
					{
						throw new InvalidDataException(
							$"{this}: Vertex color '{vertCol}' used by plate '{plateData}' was not present on the tectonic map."
						);
					}

					if (_pixelsToCoords[vertCol].Count > 1)
					{
						throw new InvalidDataException(
							$"{this}: Vertex color '{vertCol}' used by plate '{plateData}' occured multiple times on the tectonic map."
						);
					}

					vertices.Add(new PlateVertex(vertCol, _pixelsToCoords[vertCol].First()));
				}

				plates.Add(new TectonicPlateSnapshot(plateData.Name, plateData.KeyColor, vertices, _pixelsToCoords[plateData.KeyColor]));
			}

			_plates = plates;
			_plates.Sort(); // To ensure two given plates are at the same index on all snapshots, sort them.

			_plateMappings = new int[Width, Height];
			for (int p = 0; p < _plates.Count; p++)
			{
				for (int pix = 0; pix < _plates[p].Pixels.Count; pix++)
				{
					Vector2Int pixel = plates[p].Pixels[pix];
					_plateMappings[pixel.X, pixel.Y] = p;
				}
			}
		}

		/// <summary>
		/// Get a <see cref="FlowPoint"/> which starts at the given location on this snapshot,
		/// and which aims to travel towards a corresponding location on the provided snapshot.
		/// </summary>
		public FlowPoint GetTile(int x, int y, TectonicSnapshot targetSnapshot)
		{
			Vector2Int startingLocation = new Vector2Int(x, y);

			int plateIndex = _plateMappings[x, y];
			TectonicPlateSnapshot plate = _plates[plateIndex];
			TectonicPlateSnapshot targetPlate = targetSnapshot.Plates[plateIndex];

			plate.GetClosestTri(startingLocation, out PlateTriangle tri);
			Vector3 barycentric = MathUtils.CartesianToBarycentric(tri.A.Position, tri.B.Position, tri.C.Position, startingLocation);

			Vector2 targetLocation = MathUtils.BarycentricToCartesian(
				targetPlate.GetVertex(tri.A.Key).Position,
				targetPlate.GetVertex(tri.B.Key).Position,
				targetPlate.GetVertex(tri.C.Key).Position,
				barycentric
			);
			targetLocation.Floor();

			FlowPoint tile = new FlowPoint(startingLocation + Vector2.One * 0.5f, (Vector2Int)targetLocation, GetAltitude(startingLocation), tri);
			return tile;
		}

		public float GetAltitude(Vector2Int coordinate)
		{
			return _altitudes[coordinate.X, coordinate.Y];
		}

		public void DebugDraw(Scene scene)
		{
			foreach (TectonicPlateSnapshot plate in Plates)
			{
				plate.DebugDraw(scene);
			}
		}

		public override string ToString() => Name;
	}
}
