using Microsoft.Xna.Framework;
using Xunit;

namespace Amino.UnitTest
{
	public class MatrixTests
	{
		private static Matrix3x3 RotatedNinetyCL
		{
			get
			{
				Matrix3x3 rotatedNinetyCL = new Matrix3x3();
				rotatedNinetyCL.Right = -Vector2.UnitY;
				rotatedNinetyCL.Up = Vector2.UnitX;
				return rotatedNinetyCL;
			}
		}

		private static Matrix3x3 RotatedOneEightyCL
		{
			get
			{
				Matrix3x3 rotatedOneEighty = new Matrix3x3();
				rotatedOneEighty.Right = -Vector2.UnitX;
				rotatedOneEighty.Up = -Vector2.UnitY;
				return rotatedOneEighty;
			}
		}

		private static Matrix3x3 DoubledScale
		{
			get
			{
				Matrix3x3 doubled = new Matrix3x3();
				doubled.Right *= 2f;
				doubled.Up *= 2f;
				return doubled;
			}
		}

		private static Matrix3x3 RotatedNinetyCLDoubledScale
		{
			get
			{
				Matrix3x3 doubled = RotatedNinetyCL;
				doubled.Right *= 2f;
				doubled.Up *= 2f;
				return doubled;
			}
		}

		public static IEnumerable<object[]> GetMatrixDeterminantData
		{
			get
			{
				List<object[]> data = new List<object[]>();

				// Determinant of these is all 1.
				data.Add(new object[] { Matrix3x3.Identity, 1f });
				data.Add(new object[] { RotatedNinetyCL, 1f });
				data.Add(new object[] { RotatedOneEightyCL, 1f });

				// Determinant of a double-scaled matrix is 4.
				data.Add(new object[] { DoubledScale, 4f });

				return data;
			}
		}

		public static IEnumerable<object[]> GetMatrixToMatrixMultiplicationData
		{
			get
			{
				List<object[]> data = new List<object[]>();

				// Multiplying by identity gives itself.
				data.Add(new object[] { RotatedNinetyCL, Matrix3x3.Identity, RotatedNinetyCL });
				// Multiplying 90 degrees rotation by itself gives 180 rotatio.
				data.Add(new object[] { RotatedNinetyCL, RotatedNinetyCL, RotatedOneEightyCL });

				return data;
			}
		}

		public static IEnumerable<object[]> GetMatrixMinorsData
		{
			get
			{
				List<object[]> data = new List<object[]>();

				data.Add(new object[] { Matrix3x3.Identity, Matrix3x3.Identity });
				data.Add(new object[]
				{
					DoubledScale,
					new Matrix3x3(
						2f, 0f, 0f,
						0f, 2f, 0f,
						0f, 0f, 4f
					)
				});
				data.Add(new object[]
				{
					RotatedNinetyCLDoubledScale,
					new Matrix3x3(
						0f, -2f, 0f,
						2f, 0f, 0f,
						0f, 0f, 4f
					)
				});

				return data;
			}
		}

		public static IEnumerable<object[]> GetMatrixAdjointData
		{
			get
			{
				List<object[]> data = new List<object[]>();

				data.Add(new object[] { Matrix3x3.Identity, Matrix3x3.Identity });
				data.Add(new object[]
				{
					DoubledScale,
					new Matrix3x3(
						2f, 0f, 0f,
						0f, 2f, 0f,
						0f, 0f, 4f
					)
				});
				data.Add(new object[]
				{
					RotatedNinetyCLDoubledScale,
					new Matrix3x3(
						0f, -2f, 0f,
						2f, 0f, 0f,
						0f, 0f, 4f
					)
				});

				return data;
			}
		}

		[Fact]
		public void MatrixTransposeIsCorrect()
		{
			Matrix3x3 m = new Matrix3x3(
				0, 1, 2,
				3, 4, 5,
				6, 7, 8
			);

			Matrix3x3 mAdjugate = new Matrix3x3(
				0, 3, 6,
				1, 4, 7,
				2, 5, 8
			);

			Assert.Equal(mAdjugate, m.GetTransposed());
		}

		[Theory]
		[MemberData(nameof(GetMatrixMinorsData))]
		public void MatrixMinorsIsCorrect(Matrix3x3 m, Matrix3x3 result)
		{
			Assert.Equal(result, m.GetMinors());
		}

		[Theory]
		[MemberData(nameof(GetMatrixAdjointData))]
		public void MatrixAdjointIsCorrect(Matrix3x3 m, Matrix3x3 result)
		{
			Assert.Equal(result, m.GetAdjoint());
		}

		[Theory]
		[MemberData(nameof(GetMatrixDeterminantData))]
		public void MatrixDeterminantIsCorrect(Matrix3x3 m, float determinant)
		{
			Assert.Equal(determinant, m.GetDeterminant());
		}

		[Fact]
		public void MatrixInverseIsCorrect()
		{
			Matrix3x3 m = new Matrix3x3(4, 8, -2, 5, 0, 0, 12, 100, -41);
			Matrix3x3 inverse = m.GetInverse();

			Assert.Equal(Matrix3x3.Identity, m * inverse);
		}

		[Theory]
		[MemberData(nameof(GetMatrixToMatrixMultiplicationData))]
		public void MatrixToMatrixMultiplicationWorks(Matrix3x3 a, Matrix3x3 b, Matrix3x3 result)
		{
			Assert.Equal(result, a * b);
		}

		// When a parent rotates, does a 'child' (i.e. whose matrix is multiplied by, and relative to, the parent) rotate with it?
		[Fact]
		public void MatrixMultiplicationRotatesRelativeToParent()
		{
			Matrix3x3 parent = RotatedNinetyCL;
			Matrix3x3 childLocal = new Matrix3x3();
			childLocal.Translation = Vector2.One;

			Matrix3x3 childGlobal = childLocal * parent;
			Matrix3x3 expectedChildGlobal = new Matrix3x3(
				0, 1, 1,
				-1, 0, 1,
				0, 0, 1
			);

			Assert.Equal(expectedChildGlobal, childGlobal);
		}

		// If something is squashed to zero scale on one axis, but another axis remains, can the first axis be restored?
		[Fact]
		public void ZeroScaleCanBeReinferedFromPerpendicular()
		{
			Matrix3x3 m = RotatedNinetyCL;
			m.Scale = new Vector2(0f, 2f);

			Assert.Equal(Vector2.Zero, m.Right);

			m.Scale = new Vector2(1f, 2f);

			Assert.Equal(-Vector2.UnitY, m.Right);
		}

		// Zeroing a matrix's scale completely (both axes) destroys its rotational information.
		// Re-scaling it will produce a matrix whose rotation is zero (axes pointing in the cardinal directions).
		[Fact]
		public void ZeroScaleRestoresToCardinalDirections()
		{
			Matrix3x3 m = RotatedNinetyCL;
			m.Scale = Vector2.Zero;

			Assert.Equal(Vector2.Zero, m.Right);
			Assert.Equal(Vector2.Zero, m.Up);

			m.Scale = Vector2.One;

			Assert.Equal(Vector2.UnitX, m.Right);
			Assert.Equal(Vector2.UnitY, m.Up);
		}
	}
}