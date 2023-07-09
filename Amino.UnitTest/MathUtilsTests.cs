using Microsoft.Xna.Framework;
using Xunit;

namespace Amino.UnitTest
{
	/// <summary> Tests relating to <see cref="MathUtils"/>. </summary>
	public class MathUtilsTests
	{
		// Verify that the TriangleArea function gives expected results.
		[Fact]
		public void TriangleAreaWorks()
		{
			// Right angle with adjacent lengths of 1 has area half of a 1x1 square.
			float area = MathUtils.GetTriangleArea(
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f));

			Assert.Equal(0.5f, area);

			Vector2 offset = new Vector2(8f, -2.5f);
			// The same is true regardless of position.
			area = MathUtils.GetTriangleArea(
				new Vector2(0f, 0f) + offset,
				new Vector2(1f, 0f) + offset,
				new Vector2(0f, 1f) + offset);

			Assert.Equal(0.5f, area);

			// Rotation also does not change the result.
			area = MathUtils.GetTriangleArea(
				new Vector2(1f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f));

			Assert.Equal(0.5f, area);

			// An equilateral triangle has an area equal to Sqrt(3) / 4 * side-length^2.
			area = MathUtils.GetTriangleArea(
				new Vector2(0f, 0f),
				new Vector2((float)Math.Sin(MathHelper.ToRadians(60f)), 0.5f),
				new Vector2(0f, 1f));

			Assert.Equal(
				Math.Round(Math.Sqrt(3f) / 4f * 1, 2),
				Math.Round(area, 2));

			// Clockwise gives a negative area.
			area = MathUtils.GetTriangleArea(
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f));

			Assert.Equal(-0.5f, area);
		}

		// Verify that CartesianToBarycentric gives expected results.
		[Fact]
		public void CartesianToBarycentricWorks()
		{
			// Use this to verify that the same results occur even with a triangle at different locations.
			Vector2[] offsets = { Vector2.Zero, new Vector2(10f, -18f) };
			Vector3 barycentric;

			// Given a triangle, the barycentric coord of a point sitting on each corner is (1,0,0), (0,1,0), and (0,0,1).
			foreach (Vector2 offset in offsets)
			{
				barycentric = MathUtils.CartesianToBarycentric(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector2(0f, 0f) + offset
					);
				Assert.Equal(Vector3.UnitX, barycentric);

				barycentric = MathUtils.CartesianToBarycentric(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector2(1f, 0f) + offset
					);
				Assert.Equal(Vector3.UnitY, barycentric);

				barycentric = MathUtils.CartesianToBarycentric(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector2(0f, 1f) + offset
					);
				Assert.Equal(Vector3.UnitZ, barycentric);
			}

			// If we place the point outside the triangle, such that it makes a mirror image of the right angle triangle,
			// the barycentric coord will contain one element of area 2 (twice that of the triangle).
			barycentric = MathUtils.CartesianToBarycentric(
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(-1, 0f)
				);
			Assert.Equal(new Vector3(2f, -1f, 0f), barycentric);
		}

		[Fact]
		public void BarycentricToCartesianWorks()
		{
			// Use this to verify that the same results occur even with a triangle at different locations.
			Vector2[] offsets = { Vector2.Zero, new Vector2(10f, -18f) };
			Vector2 cartesian;

			foreach (Vector2 offset in offsets)
			{
				cartesian = MathUtils.BarycentricToCartesian(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector3(1f, 0f, 0f)
					);
				Assert.Equal(new Vector2(0f, 0f) + offset, cartesian);

				cartesian = MathUtils.BarycentricToCartesian(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector3(0f, 1f, 0f)
					);
				Assert.Equal(new Vector2(1f, 0f) + offset, cartesian);

				cartesian = MathUtils.BarycentricToCartesian(
					new Vector2(0f, 0f) + offset,
					new Vector2(1f, 0f) + offset,
					new Vector2(0f, 1f) + offset,
					new Vector3(0f, 0f, 1f)
					);
				Assert.Equal(new Vector2(0f, 1f) + offset, cartesian);
			}

			// Non-normalised coords are also accepted.
			cartesian = MathUtils.BarycentricToCartesian(
					new Vector2(0f, 0f),
					new Vector2(1f, 0f),
					new Vector2(0f, 1f),
					new Vector3(2f, 0f, 0f)
					);
			Assert.Equal(new Vector2(0f, 0f), cartesian);
		}

		[Fact]
		public void AngleTest()
		{
			Assert.Equal(90f, MathUtils.Angle(Vector2.UnitY, Vector2.UnitX));
			Assert.Equal(-180f, MathUtils.Angle(Vector2.UnitY, -Vector2.UnitY));
			Assert.Equal(-90f, MathUtils.Angle(Vector2.UnitY, -Vector2.UnitX));
			Assert.Equal(0f, MathUtils.Angle(Vector2.UnitY, Vector2.UnitY));
		}

		[Fact]
		public void PolygonClockwiseTest()
		{
			Assert.False(MathUtils.IsClockwise(new List<Vector2>()
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
			}));
			Assert.True(MathUtils.IsClockwise(new List<Vector2>()
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
			}));

			// Shape of no area is considered clockwise.
			Assert.True(MathUtils.IsClockwise(new List<Vector2>()
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
			}));

			// Self intersecting shape with the larger area being to the left of the edge is considered counter-clockwise.
			Assert.False(MathUtils.IsClockwise(new List<Vector2>()
			{
				new Vector2(0f, 0f),
				new Vector2(4f, 0f),
				new Vector2(1f, 1f),
				new Vector2(1f, 2f),
			}));
		}
	}
}
