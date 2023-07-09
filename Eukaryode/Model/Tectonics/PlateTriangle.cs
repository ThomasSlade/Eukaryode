using Amino;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Eukaryode.Tectonics
{
	/// <summary> A triangle within a plate's geometry, defined by three <see cref="PlateVertex"/> elements in counter-clockwise order. </summary>
	public struct PlateTriangle
	{
		public Color DebugColor { get; init; }
		public PlateVertex A { get; private init; }
		public PlateVertex B { get; private init; }
		public PlateVertex C { get; private init; }

		public PlateTriangle(PlateVertex a, PlateVertex b, PlateVertex c)
		{
			DebugColor = AminoUtils.RandomColor();
			A = a;
			B = b;
			C = c;
		}

		public override bool Equals([NotNullWhen(true)] object? obj)
			=> obj is PlateTriangle other && other.A == A && other.B == B && other.C == C;

		public override int GetHashCode()
			=> HashCode.Combine(A, B, C);

		public static bool operator ==(PlateTriangle left, PlateTriangle right)
			=> left.Equals(right);

		public static bool operator !=(PlateTriangle left, PlateTriangle right)
			=> !(left == right);

		public override string ToString() => DebugColor.ToString();
	}
}
