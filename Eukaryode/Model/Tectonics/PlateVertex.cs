using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Eukaryode.Tectonics
{
	/// <summary> A corner of a tectonic plate, identified by a unique key-colour. </summary>
	public struct PlateVertex
	{
		public Color Key { get; private init; }
		public Vector2Int Position { get; private init; }

		public PlateVertex(Color key, Vector2Int position)
		{
			Key = key;
			Position = position;
		}

		public override bool Equals([NotNullWhen(true)] object? obj)
			=> obj is PlateVertex v && Key == v.Key && Position == v.Position;

		public override int GetHashCode()
			=> HashCode.Combine(Key, Position);

		public static bool operator ==(PlateVertex a, PlateVertex b)
			=> a.Equals(b);

		public static bool operator !=(PlateVertex a, PlateVertex b)
			=> !a.Equals(b);
	}
}
