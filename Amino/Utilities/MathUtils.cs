using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace Amino
{
	/// <summary> Utilities relating to math. </summary>
	public static class MathUtils
	{
		/// <summary> Function which, unlike '%', returns 1 for -1 % 10. </summary>
		public static int Mod(int a, int m) => (a % m + m) % m;

		/// <summary> Function which, unlike '%', returns 1 for -1 % 10. </summary>
		public static float Mod(float a, float m) => (a % m + m) % m;

		/// <summary> Convert a flattened 1D index into its 2D coordinates, given the dimensions of a 2D grid. </summary>
		public static Vector2Int UnflattenIndex(int index, int gridWidth, int gridHeight)
			=> new Vector2Int(index % gridWidth, index / gridWidth);

		/// <summary>
		/// Convert a cartesian point to its barycentric coordinate relative to a triangle.
		/// The triangle's points must be in counter-clockwise order.
		/// </summary>
		/// <returns>
		/// Returns a Vector 3 coordinate where:
		/// X is the mass of triA,
		/// Y is the mass of triB,
		/// Z is the mass of triC.
		/// </returns>
		public static Vector3 CartesianToBarycentric(Vector2 triA, Vector2 triB, Vector2 triC, Vector2 cartesian)
		{
			float triArea = GetTriangleArea(triA, triB, triC);
			if(triArea < 0f)
			{
				throw new ArgumentException($"Triangle area was found to be a negative number. The triangle points provided must be in counter-clockwise order.");
			}
			float w = GetTriangleArea(triA, triB, cartesian);
			float v = GetTriangleArea(triA, cartesian, triC);
			float u = triArea - (w + v);

			float normalizeFactor = 1f / triArea;

			return new Vector3(u, v, w) * normalizeFactor;
		}

		/// <summary>
		/// Convert a barycentric coordinate relative to a triangle to the cartesian point it represents.
		/// The triangle's points must be in counter-clockwise order.
		/// </summary>
		public static Vector2 BarycentricToCartesian(Vector2 triA, Vector2 triB, Vector2 triC, Vector3 barycentric)
		{
			return barycentric.X * triA + barycentric.Y * triB + barycentric.Z * triC;
		}

		/// <summary>
		/// Get the signed angle that <paramref name="v1"/> must rotate, clockwise, to align with <paramref name="v2"/> (in degrees).
		/// A value greater than 180 or -180 is never returned.
		/// </summary>
		public static float Angle(Vector2 v1, Vector2 v2)
		{
			float dot = v1.Dot(v2);
			float determinant = v1.X * v2.Y - v1.Y * v2.X;
			return -MathHelper.ToDegrees((float)Math.Atan2(determinant, dot));
		}

		/// <summary>
		/// Determine if the provided list of vertices run clockwise or counterclockwise.
		/// This is done by determining if the enclosed area is positive or negative. For self-intersecting polygons, a 'mostly positive'
		/// i.e. mostly-internal area will still be considered clockwise.
		/// A shape with an area of 0 is considered clockwise.
		/// </summary>
		public static bool IsClockwise(IList<Vector2> polygon) => GetPolygonArea(polygon) <= 0f;

		/// <summary>
		/// Get the area of the specified triangle.
		/// Invokes <see cref="GetPolygonArea(IList{Vector2})"/>.
		/// </summary>
		public static float GetTriangleArea(Vector2 a, Vector2 b, Vector2 c) => GetPolygonArea(new Vector2[] { a, b, c });

		/// <summary>
		/// Get the area of a polygon with no holes in it.
		/// If the polygon's vertices are in clockwise order, the area will be positive, and will otherwise be negative.
		/// A self-intersecting polygon's area will be the sum of all area on the right and left side of its edge,
		/// so that a self-intersecting polygin that is 'mostly' clockwise will still return a positive area.
		/// </summary>
		public static float GetPolygonArea(IList<Vector2> polygon)
		{
			float sum = 0f;
			for (int p = 0; p < polygon.Count; p++)
			{
				Vector2 current = polygon[p];
				Vector2 next = polygon[(p + 1) % polygon.Count];

				sum += (next.X - current.X) * (next.Y + current.Y);
			}
			return -sum * 0.5f;
		}

		/// <summary> Determine if <paramref name="point"/> falls within the counter-clockwise triangle. </summary>
		public static bool TriangleContainsPoint(Vector2 triA, Vector2 triB, Vector2 triC, Vector2 point)
		{
			Vector3 barycentric = CartesianToBarycentric(triA, triB, triC, point);
			return barycentric.X >= 0f && barycentric.Y >= 0f && barycentric.Z >= 0f;
		}

		/// <summary> Get the distance of the point to any edge of the counter-clockwise triangle. If inside the triangle, returns negative. </summary>
		public static float DistanceFromTriangle(Vector2 triA, Vector2 triB, Vector2 triC, Vector2 point)
		{
			Vector2 ab = triB - triA;
			Vector2 bc = triC - triB;
			Vector2 ca = triA - triC;

			Vector2 aP = point - triA;
			Vector2 bP = point - triB;
			Vector2 cP = point - triC;

			Vector2 d = aP - ab * Math.Clamp(aP.Dot(ab) / ab.Dot(ab), 0f, 1f);
			Vector2 e = bP - bc * Math.Clamp(bP.Dot(bc) / bc.Dot(bc), 0f, 1f);
			Vector2 f = cP - ca * Math.Clamp(cP.Dot(ca) / ca.Dot(ca), 0f, 1f);

			float s = ab.X * ca.Y - ab.Y * ca.X;

			Vector2 dist = Vector2.Min(
				new Vector2(d.Dot(d), s * (aP.X * ab.Y - aP.Y * ab.X)),
				new Vector2(e.Dot(e), s * (bP.X * bc.Y - bP.Y * bc.X))
			);

			dist = Vector2.Min(
			   dist,
			   new Vector2(f.Dot(f), s * (cP.X * ca.Y - cP.Y * ca.X))
			);

			return Math.Sign(dist.Y) * -(float)Math.Sqrt(dist.X);
		}
	}
}
