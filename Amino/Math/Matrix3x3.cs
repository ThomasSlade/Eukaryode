using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Amino
{
	/// <summary> A 3x3 matrix that can be used to represent a 2D transform. </summary>
	public struct Matrix3x3 : IEquatable<Matrix3x3>
	{
		public float M11;
		public float M12;
		public float M13;
		public float M21;
		public float M22;
		public float M23;
		public float M31;
		public float M32;
		public float M33;

		/// <summary>
		/// Get an element of this matrix by index.
		/// Indices move along the rows, then down the columns, of the matrix.
		/// </summary>
		public float this[int index]
		{
			get
			{
				return index switch
				{
					0 => M11,
					1 => M12,
					2 => M13,
					3 => M21,
					4 => M22,
					5 => M23,
					6 => M31,
					7 => M32,
					8 => M33,
					_ => throw new IndexOutOfRangeException($"Index '{index}' out of Matrix 3x3 range (must be >= 0 and <= 8).")
				};
			}
			set
			{
				switch (index)
				{
					case 0: M11 = value; break;
					case 1: M12 = value; break;
					case 2: M13 = value; break;
					case 3: M21 = value; break;
					case 4: M22 = value; break;
					case 5: M23 = value; break;
					case 6: M31 = value; break;
					case 7: M32 = value; break;
					case 8: M33 = value; break;
					default: throw new IndexOutOfRangeException($"Index '{index}' out of Matrix 3x3 range (must be >= 0 and <= 8).");
				}
			}
		}

		/// <summary>
		/// Get an element of this matrix by its vertical, and then horizontal position.
		/// Note that this means coordinates follow an (upwards, sideways) format, unlike what's typical in graphs where X is sideways and Y is upwards.
		/// </summary>
		public float this[int row, int column]
		{
			get
			{
				if (row < 0 || row > 2 || column < 0 || column > 2)
				{
					throw new IndexOutOfRangeException($"Cannot access element of Matrix 3x3 with '{row}, {column}': both row and column indices must be >= 0 and <= 2");
				}
				return this[row * 3 + column];
			}
			set
			{
				if (row < 0 || row > 2 || column < 0 || column > 2)
				{
					throw new IndexOutOfRangeException($"Cannot access element of Matrix 3x3 with '{row}, {column}': both row and column indices must be >= 0 and <= 2");
				}
				this[row * 3 + column] = value;
			}
		}

		/// <summary> The identity matrix. </summary>
		public static Matrix3x3 Identity { get; private set; } = new Matrix3x3(
			1f, 0f, 0f,
			0f, 1f, 0f,
			0f, 0f, 1f
		);

		/// <summary> This matrix's rightward axis. </summary>
		public Vector2 Right
		{
			get => new Vector2(M11, M21);
			set
			{
				M11 = value.X;
				M21 = value.Y;
			}
		}

		/// <summary> The reverse of <see cref="Right"/>. </summary>
		public Vector2 Left
		{
			get => -Right;
			set => Right = -value;
		}

		/// <summary> This matrix's upwards axis. </summary>
		public Vector2 Up
		{
			get => new Vector2(M12, M22);
			set
			{
				M12 = value.X;
				M22 = value.Y;
			}
		}

		/// <summary> The reverse of <see cref="Up"/>. </summary>
		public Vector2 Down
		{
			get => -Up;
			set => Up = -value;
		}

		/// <summary> The translation of this matrix. </summary>
		public Vector2 Translation
		{
			get => new Vector2(M13, M23);
			set
			{
				M13 = value.X;
				M23 = value.Y;
			}
		}

		/// <summary>
		/// The translation of this matrix, relative to its rotation/scale element.
		/// That is, a relative translation of (0, 5) would signify 5 units upward in the matrix's local upwards direction.
		/// </summary>
		public Vector2 RelativeTranslation
		{
			get
			{
				Matrix3x3 rotationMatrix = this;
				rotationMatrix.Scale = Vector2.One;
				rotationMatrix.Translation = Vector2.Zero;
				Vector2 translationRelativeToBearing = rotationMatrix * Translation;
				return translationRelativeToBearing;
			}
			set
			{
				Matrix3x3 rotationMatrix = this;
				rotationMatrix.Scale = Vector2.One;
				rotationMatrix.Translation = Vector2.Zero;
				Translation = rotationMatrix.GetInverse() * value;
			}
		}

		/// <summary> The scale of this matrix. </summary>
		public Vector2 Scale
		{
			get => new Vector2(Right.Length(), Up.Length());
			set
			{
				Vector2 currentScale = Scale;
				bool recalculateCurrentScale = false;

				if(value.X != 0 && currentScale.X == 0)
				{
					if (currentScale.Y != 0)
					{
						Right = Up.Perpendicular(true);
						Right.Normalize();
					}
					else
					{
						Right = Vector2.UnitX;
					}
					Right *= 0.001f;
					recalculateCurrentScale = true;
				}

				if(value.Y != 0 && currentScale.Y == 0)
				{
					Up = Right.Perpendicular(false) * 0.001f;
					recalculateCurrentScale = true;
				}

				if (recalculateCurrentScale)
				{
					currentScale = Scale;
				}

				Right *= value.X / currentScale.X;
				Up *= value.Y / currentScale.Y;
			}
		}

		/// <summary> The rotation of this matrix, in degrees. </summary>
		public float Rotation
		{
			get => (float)Math.Atan2(M21, M11);
			set
			{
				float currentRotation = Rotation;
				value -= currentRotation;
				float sinRoll = (float)Math.Sin(value);
				float cosRoll = (float)Math.Cos(value);
				Matrix3x3 rotation = new Matrix3x3();
				rotation.M11 = cosRoll;
				rotation.M21 = -sinRoll;
				rotation.M12 = sinRoll;
				rotation.M22 = cosRoll;
				this *= rotation;
			}
		}

		/// <summary> Get a string representing this matrix in a grid. </summary>
		internal string DebugDisplayString
			=> this == Identity ? "Identity" : $"({M11}, {M12}, {M13})\n({M21}, {M22}, {M23})\n({M31}, {M32}, {M33})";

		public Matrix3x3()
		{
			this = Identity;
		}

		public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)  : this()
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M31 = m31;
			M32 = m32;
			M33 = m33;
		}

		public Matrix3x3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
			: this((float)m11, (float)m12, (float)m13, (float)m21, (float)m22, (float)m23, (float)m31, (float)m32, (float)m33)
		{

		}

		public Matrix3x3(Matrix3x3 matrix)
			: this(matrix.M11, matrix.M12, matrix.M13, matrix.M21, matrix.M22, matrix.M23, matrix.M31, matrix.M32, matrix.M33)
		{

		}

		/// <summary> Get the minor of the specified element. </summary>
		public float GetMinor(int row, int column)
		{
			float[] operands = new float[4];
			int operandIndex = 0;

			List<Vector2> operandCoords = new List<Vector2>();

			for (int c = 0; c < 3; c++)
			{
				if (c == column)
					continue;

				for (int r = 0; r < 3; r++)
				{
					if (r == row)
						continue;

					operands[operandIndex++] = this[r, c];
					operandCoords.Add(new Vector2(r, c));
				}
			}

			return operands[0] * operands[3] - operands[1] * operands[2];
		}

		/// <summary> Get a matrix of minors from this matrix. </summary>
		public Matrix3x3 GetMinors()
		{
			Matrix3x3 matrixOfMinors = new Matrix3x3();
			for (int c = 0; c < 3; c++)
			{
				for (int r = 0; r < 3; r++)
				{
					matrixOfMinors[c, r] = GetMinor(c, r);
				}
			}
			return matrixOfMinors;
		}

		/// <summary> Get a matrix of cofactors from this matrix. </summary>
		public Matrix3x3 GetCofactors()
		{
			Matrix3x3 matrixOfCofactors = GetMinors();
			for (int i = 1; i < 9; i += 2)
			{
				matrixOfCofactors[i] = -matrixOfCofactors[i];
			}
			return matrixOfCofactors;
		}

		/// <summary> Flip this matrix's elements about the downward-right diagonal. </summary>
		public Matrix3x3 GetTransposed()
		{
			Matrix3x3 adjugated = new Matrix3x3(this);
			adjugated.M12 = this.M21;
			adjugated.M21 = this.M12;
			adjugated.M13 = this.M31;
			adjugated.M31 = this.M13;
			adjugated.M23 = this.M32;
			adjugated.M32 = this.M23;
			return adjugated;
		}

		/// <summary> Get the adoint of this matrix. </summary>
		public Matrix3x3 GetAdjoint()
		{
			return GetCofactors().GetTransposed();
		}

		/// <summary> Get this matrix's determinant. </summary>
		public float GetDeterminant()
			=> M11 * GetMinor(0, 0) - M12 * GetMinor(0, 1) + M13 * GetMinor(0, 2);

		/// <summary> Get this matrix's inverse. </summary>
		public Matrix3x3 GetInverse()
			=> GetAdjoint() * 1f / GetDeterminant();

		/// <summary> Get a copy of this matrix, with elements rounded to the provided decimals. </summary>
		public Matrix3x3 Round(int decimals)
		{
			return new Matrix3x3(
				Math.Round(M11, decimals), Math.Round(M12, decimals), Math.Round(M13, decimals),
				Math.Round(M21, decimals), Math.Round(M22, decimals), Math.Round(M23, decimals),
				Math.Round(M31, decimals), Math.Round(M32, decimals), Math.Round(M33, decimals)
			);
		}

        public bool Equals(Matrix3x3 other)
        {
            return M11 == other.M11
                && M12 == other.M12
                && M13 == other.M13
                && M21 == other.M21
                && M22 == other.M22
                && M23 == other.M23
                && M31 == other.M31
                && M32 == other.M32
                && M33 == other.M33;
        }

		public override bool Equals([NotNullWhen(true)] object? obj)
			=> obj is Matrix3x3 otherMatrix ? Equals(otherMatrix) : false;

		public override string ToString() => $"({M11}, {M12}, {M13}) ({M21}, {M22}, {M23}) ({M31}, {M32}, {M33})";

		/// <summary> Get a matrix that can be multiplied against to scale another matrix. </summary>
		public static Matrix3x3 ScaleMatrix(Vector2 scale)
		{
			Matrix3x3 m = new Matrix3x3();
			m.Scale = scale;
			return m;
		}

		/// <summary> Get a matrix that can be multiplied against to transform another matrix. </summary>
		public static Matrix3x3 TranslationMatrix(Vector2 translation)
		{
			Matrix3x3 m = new Matrix3x3();
			m.Translation = translation;
			return m;
		}

		public static bool operator ==(Matrix3x3 a, Matrix3x3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Matrix3x3 a, Matrix3x3 b)
        {
            return !a.Equals(b);
        }

        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            // First row.
            float p11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            float p12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            float p13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;

            // Second row.
            float p21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            float p22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            float p23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;

            // Third row.
            float p31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            float p32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            float p33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;

            a.M11 = p11;
            a.M12 = p12;
            a.M13 = p13;
            a.M21 = p21;
            a.M22 = p22;
            a.M23 = p23;
            a.M31 = p31;
			a.M32 = p32;
			a.M33 = p33;

			return a;
        }

		public static Matrix3x3 operator *(Matrix3x3 matrix, float scalar)
		{
			matrix.M11 *= scalar;
			matrix.M12 *= scalar;
			matrix.M13 *= scalar;
			matrix.M21 *= scalar;
			matrix.M22 *= scalar;
			matrix.M23 *= scalar;
			matrix.M31 *= scalar;
			matrix.M32 *= scalar;
			matrix.M33 *= scalar;
			return matrix;
		}

		public static Matrix3x3 operator /(Matrix3x3 matrix, float scalar) => matrix * (1f / scalar);

		public static Vector2 operator *(Matrix3x3 matrix, Vector2 vector)
		{
			return new Vector2(
				matrix.M11 * vector.X + matrix.M21 * vector.Y,
				matrix.M12 * vector.X + matrix.M22 * vector.Y
			);
		}

		public static Matrix3x3 operator +(Matrix3x3 a, Matrix3x3 b)
		{
			a.M11 += b.M11;
			a.M12 += b.M12;
			a.M13 += b.M13;
			a.M21 += b.M21;
			a.M22 += b.M22;
			a.M21 += b.M23;
			a.M31 += b.M31;
			a.M32 += b.M32;
			a.M33 += b.M33;
			return a;
		}

		public static Matrix3x3 operator -(Matrix3x3 a, Matrix3x3 b)
		{
			a.M11 -= b.M11;
			a.M12 -= b.M12;
			a.M13 -= b.M13;
			a.M21 -= b.M21;
			a.M22 -= b.M22;
			a.M21 -= b.M23;
			a.M31 -= b.M31;
			a.M32 -= b.M32;
			a.M33 -= b.M33;
			return a;
		}

		public static Matrix3x3 operator-(Matrix3x3 matrix)
		{
			matrix.M11 = -matrix.M11;
			matrix.M12 = -matrix.M12;
			matrix.M13 = -matrix.M13;
			matrix.M21 = -matrix.M21;
			matrix.M22 = -matrix.M22;
			matrix.M23 = -matrix.M23;
			matrix.M31 = -matrix.M31;
			matrix.M32 = -matrix.M32;
			matrix.M33 = -matrix.M33;
			return matrix;
		}
	}
}
