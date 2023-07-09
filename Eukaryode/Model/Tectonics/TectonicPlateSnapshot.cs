using Amino;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Eukaryode.Tectonics
{
	/// <summary> An individual plate within <see cref="TectonicSnapshot"/>, which is defined by a set of positions on the map (vertices). </summary>
	public class TectonicPlateSnapshot : IComparable<TectonicPlateSnapshot>
    {
		public string Name { get; init; }

		public Color DebugColor { get; init; }

		/// <summary> The triangles defining this plate. </summary>
		public IReadOnlyList<PlateTriangle> Triangles => _triangles;
		private List<PlateTriangle> _triangles;

		/// <summary> The vertices mapped by their key colour. </summary>
		private Dictionary<Color, PlateVertex> _verticesByKey;

		/// <summary> The pixel coordinates that constitute the geometry of this plate. </summary>
		public IReadOnlyList<Vector2Int> Pixels => _pixels;
		private List<Vector2Int> _pixels = new();

		public TectonicPlateSnapshot(string name, Color debugColor, IList<PlateVertex> vertices, IEnumerable<Vector2Int> pixels)
		{
			Name = name;
			DebugColor = debugColor;

			Vector2[] polygon = vertices.Select(v => (Vector2)v.Position).ToArray();
			int[] triIndices = EarClipper.Triangulate(polygon);

			_triangles = new List<PlateTriangle>(triIndices.Length / 3);
			for (int t = 0; t < triIndices.Length; t += 3)
			{
				_triangles.Add(new PlateTriangle(vertices[triIndices[t]], vertices[triIndices[t + 1]], vertices[triIndices[t + 2]]));
			}

			_verticesByKey = new Dictionary<Color, PlateVertex>(vertices.Count);
			foreach (PlateVertex vertex in vertices)
			{
				_verticesByKey.Add(vertex.Key, vertex);
			}

			_pixels.AddRange(vertices.Select(v => v.Position));
			_pixels.AddRange(pixels);
		}

		public void DebugDraw(Scene scene)
		{
			foreach (PlateTriangle triangle in Triangles)
			{
				scene.DebugDraw(new DebugLine(triangle.A.Position, triangle.B.Position, triangle.DebugColor));
				scene.DebugDraw(new DebugLine(triangle.B.Position, triangle.C.Position, triangle.DebugColor));
				scene.DebugDraw(new DebugLine(triangle.C.Position, triangle.A.Position, triangle.DebugColor));
			}
		}

		public PlateVertex GetVertex(Color key) => _verticesByKey[key];

		/// <summary>
		/// Get the triangle of this plate that contains, or is nearest to, this point.
		/// Nearness is defined as the point having the barycentric coordinate for the given triangle that,
		/// with its element as absolute values, is closest to 1.
		/// </summary>
		public void GetClosestTri(Vector2 point, out PlateTriangle tri)
		{
			float shortestDist = float.MaxValue;
			tri = Triangles.First();

			for (int t = 0; t < Triangles.Count; t++)
			{
				PlateTriangle currentTri = Triangles[t];
				float dist = MathUtils.DistanceFromTriangle(currentTri.A.Position, currentTri.B.Position, currentTri.C.Position, point);

				if(dist <= 0f)
				{
					tri = currentTri;
					return;
				}

				if(dist < shortestDist)
				{
					shortestDist = dist;
					tri = currentTri;
				}
			}
		}

		public int CompareTo(TectonicPlateSnapshot? other) => Name.CompareTo(other.Name);
	}
}
