using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework;

/// <summary>
/// A vector which can only have integer components.
/// </summary>
/// <remarks>
/// Based on the <see cref="Vector2"/> class's source code.
/// </remarks>
public struct Vector2Int
{
	private static readonly Vector2Int ZeroVector = new Vector2Int(0, 0);
	private static readonly Vector2Int UnitVector = new Vector2Int(1, 1);
	private static readonly Vector2Int UnitXVector = new Vector2Int(1, 0);
	private static readonly Vector2Int UnitYVector = new Vector2Int(0, 1);

	/// <summary> The x coordinate of this <see cref="Vector2Int"/>. </summary>
	public int X;

	/// <summary> The y coordinate of this <see cref="Vector2Int"/>. </summary>
	public int Y;

	/// <summary> Returns a <see cref="Vector2Int"/> with components 0, 0. </summary>
	public static Vector2Int Zero => ZeroVector;
	/// <summary> Returns a <see cref="Vector2Int"/> with components 1, 1. </summary>
	public static Vector2Int One => UnitVector;
	/// <summary> Returns a <see cref="Vector2Int"/> with components 1, 0. </summary>
	public static Vector2Int UnitX => UnitXVector;
	/// <summary> Returns a <see cref="Vector2Int"/> with components 0, 1. </summary>
	public static Vector2Int UnitY => UnitYVector;

	public Vector2Int(int x, int y)
	{
		X = x;
		Y = y;
	}

	public Vector2Int(int value)
	{
		X = value;
		Y = value;
	}

	/// <summary> Inverts the value. </summary>
	public static Vector2Int operator -(Vector2Int value)
	{
		value.X = -value.X;
		value.Y = -value.Y;
		return value;
	}

	/// <summary> Adds two vectors. </summary>
	public static Vector2Int operator +(Vector2Int value1, Vector2Int value2)
	{
		value1.X += value2.X;
		value1.Y += value2.Y;
		return value1;
	}

	/// <summary> Adds two vectors, casting the <see cref="Vector2Int"/> to a <see cref="Vector2"/> in the process. </summary>
	public static Vector2 operator +(Vector2Int value1, Vector2 value2)
	{
		Vector2 asV2 = (Vector2)value1;
		asV2.X += value2.X;
		asV2.Y += value2.Y;
		return asV2;
	}

	/// <summary> Adds two vectors. </summary>
	public static Vector2 operator +(Vector2 value1, Vector2Int value2)
	{
		value1.X += value2.X;
		value1.Y += value2.Y;
		return value1;
	}

	/// <summary> Subtracts two vectors. </summary>
	public static Vector2Int operator -(Vector2Int value1, Vector2Int value2)
	{
		value1.X -= value2.X;
		value1.Y -= value2.Y;
		return value1;
	}

	/// <summary> Subtracts two vectors, casting the <see cref="Vector2Int"/> to a <see cref="Vector2"/> in the process. </summary>
	public static Vector2 operator -(Vector2Int value1, Vector2 value2)
	{
		Vector2 asV2 = (Vector2)value1;
		asV2.X -= value2.X;
		asV2.Y -= value2.Y;
		return asV2;
	}

	/// <summary> Subtracts two vectors. </summary>
	public static Vector2 operator -(Vector2 value1, Vector2Int value2)
	{
		value1.X -= value2.X;
		value1.Y -= value2.Y;
		return value1;
	}

	/// <summary> Multiplies the components of two vectors by each other. </summary>
	public static Vector2Int operator *(Vector2Int value1, Vector2Int value2)
	{
		value1.X *= value2.X;
		value1.Y *= value2.Y;
		return value1;
	}

	/// <summary> Multiplies a vector by a scalar. </summary>
	public static Vector2Int operator *(Vector2Int value, int scalar)
	{
		value.X *= scalar;
		value.Y *= scalar;
		return value;
	}

	/// <summary> Multiplies a vector by a scalar. </summary>
	public static Vector2Int operator *(int scalar, Vector2Int value)
	{
		value.X *= scalar;
		value.Y *= scalar;
		return value;
	}

	/// <summary> Divides the components of a vector by the components of another vector. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator /(Vector2Int value1, Vector2Int value2)
	{
		value1.X /= value2.X;
		value1.Y /= value2.Y;
		return value1;
	}

	/// <summary> Divides a vector by a scalar. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator /(Vector2Int value, int divider)
	{
		value.X /= divider;
		value.Y /= divider;
		return value;
	}

	/// <summary> Divides a vector by a scalar. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int operator /(int divider, Vector2Int value)
	{
		value.X /= divider;
		value.Y /= divider;
		return value;
	}

	/// <summary> Casts the <see cref="Vector2Int"/> to a <see cref="Vector2"/> </summary>
	public static implicit operator Vector2(Vector2Int value)
		=> new Vector2(value.X, value.Y);

	/// <summary> Casts the <see cref="Vector2"/> to a <see cref="Vector2Int"/> </summary>
	public static explicit operator Vector2Int(Vector2 value)
		=> new Vector2Int((int)value.X, (int)value.Y);

	/// <summary> Compares whether two vectors are equal. </summary>
	public static bool operator ==(Vector2Int value1, Vector2Int value2) => value1.X == value2.X && value1.Y == value2.Y;

	/// <summary> Compares whether two vectors are not equal. </summary>
	public static bool operator !=(Vector2Int value1, Vector2Int value2) => value1.X != value2.X || value1.Y != value2.Y;

	/// <summary> Returns the distance between two vectors. </summary>
	public static float Distance(Vector2Int value1, Vector2Int value2)
	{
		float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
		return MathF.Sqrt(v1 * v1 + v2 * v2);
	}

	/// <summary> Returns the distance squared between two vectors. </summary>
	public static float DistanceSquared(Vector2Int value1, Vector2Int value2)
	{
		float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
		return v1 * v1 + v2 * v2;
	}

	/// <summary> Returns the length of this vector. </summary>
	public float Length() => MathF.Sqrt(X * X + Y * Y);

	/// <summary> Returns the squared length of this vector. </summary>
	public float LengthSquared() => X * X + Y * Y;

	/// <summary> Returns the dot product of two vectors. </summary>
	public static float Dot(Vector2Int value1, Vector2Int value2)
		=> value1.X * value2.X + value1.Y * value2.Y;

	/// <summary> Compares whether current instance is equal to specified <see cref="Vector2Int"/>. </summary>
	public bool Equals(Vector2Int other)
		=> X == other.X && Y == other.Y;

	/// <summary> Compares whether current instance is equal to specified <see cref="Object"/>. </summary>
	public override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is Vector2Int v2Int ? Equals(v2Int) : false;

	/// <summary> Gets the hash code of this <see cref="Vector2Int"/>. </summary>
	public override int GetHashCode()
	{
		unchecked
		{
			return (X.GetHashCode() * 251) ^ Y.GetHashCode();
		}
	}

	public override string ToString() => "{X:" + X + " Y:" + Y + "}";
}
