using Microsoft.Xna.Framework;
using Xunit;

namespace Amino.UnitTest
{
	/// <summary> Tests for the ear-clipping triangulation algorithm. </summary>
	public class EarClipperTests
	{
		public static IEnumerable<object[]> Polygons =>
			new List<object[]>
			{
				new object[]
				{
					// Square
					new Vector2[]
					{
						new(0f, 0f),
						new(1f, 0f),
						new(1f, 1f),
						new(0f, 1f)
					}
				},
				new object[]
				{
					// L Shape
					new Vector2[]
					{
						new(0f, 0f),
						new(1f, 0f),
						new(1f, 0.5f),
						new(0.5f, 0.5f),
						new(0.5f, 1f),
						new(0f, 1f)
					}
				},
				new object[]
				{
					// Rotated L Shape
					new Vector2[]
					{
						new(0.5f, 0.5f),
						new(0.5f, 0f),
						new(1f, 0f),
						new(1f, 1f),
						new(0f, 1f),
						new(0f, 0.5f)
					}
				},
				new object[]
				{
					// Z Shape
					new Vector2[]
					{
						new(0f, 0f),
						new(1f, 0f),
						new(1f, 0.5f),
						new(2f, 0.5f),
						new(2f, 1f),
						new(0.5f, 1f),
						new(0.5f, 0.5f),
						new(0f, 0.5f)
					}
				}
			};

		[Theory]
		[MemberData(nameof(Polygons))]
		public void TriangleCountAndAreaTest(Vector2[] polygon)
		{
			int[] tris = EarClipper.Triangulate(polygon);
			Assert.Equal((polygon.Length - 2) * 3, tris.Length);
			Assert.Equal(MathUtils.GetPolygonArea(polygon), GetTriangleAreas(polygon, tris));
		}

		private float GetTriangleAreas(Vector2[] polygon, int[] tris)
		{
			if(tris.Length != (polygon.Length - 2) * 3)
			{
				throw new ArgumentException($"The provided triangles do not contain the expected number of tri vertices for a polygon with '{polygon.Length}' corners ('{(polygon.Length - 2) * 3}' expected, but got '{tris.Length}').");
			}

			float sumArea = 0f;
			for (int t = 0; t < tris.Length; t += 3)
			{
				sumArea += MathUtils.GetTriangleArea(polygon[tris[t]], polygon[tris[t + 1]], polygon[tris[t + 2]]);
			}
			return sumArea;
		}
	}
}
