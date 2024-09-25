using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Amino
{
	/// <summary>
	/// Takes a simple, non-self-intersecting polygon defined by a list of positions counter-clockwise positions, and determines its triangles.
	/// </summary>
	public class EarClipper
	{
		/// <summary>
		/// The vertices defining the polygon to be triangulated. This does not change as the algorithm executes.
		/// Indices in <see cref="_clippedPolygon"/> and elsewhere refer to these vertices.
		/// </summary>
		private IList<Vector2> _originalPolygon;
		/// <summary>
		/// The current polygon which iteratively has its ears removed.
		/// This is a list of the indicies of <see cref="_originalPolygon"/>, and thus starts as a list { 0, 1, ... _originalPolygon.Count - 1 }
		/// </summary>
		private LinkedList<int> _clippedPolygon;
		/// <summary> How many verts are left in <see cref="_clippedPolygon"/>. </summary>
		private int _remainingVertCount;
		/// <summary>
		/// Any verts which have been found to be 'ears'
		/// (verts with a convex angle, which contain no other verts within the triangle formed by them and their neighbors).
		/// </summary>
		private HashSet<LinkedListNode<int>> _ears = new HashSet<LinkedListNode<int>>();
		/// <summary> Any verts which have been found to have an angle of greater than 180 degrees. </summary>
		private HashSet<int> _concave = new HashSet<int>();

		/// <summary> The results of the triangulation. Each three elements represents a triangle, by its corner's indices in <see cref="_originalPolygon"/>. </summary>
		List<int> _triangles;
		/// <summary> How many triangles were expected given the initial vertex count: this is always (vertex count) - 2 * 3 </summary>
		private int _expectedTriCount;

		public EarClipper(IList<Vector2> polygon)
		{
			if (polygon.Count < 3)
			{
				throw new ArgumentException($"Cannot triangulate a polygon that has vertex count ('{polygon.Count}') of less than 3.");
			}

			_originalPolygon = polygon;
			_remainingVertCount = polygon.Count;
			_expectedTriCount = (polygon.Count - 2) * 3;
			_triangles = new List<int>(_expectedTriCount);
			Triangulate();
		}

		/// <summary> Run the triangulation. </summary>
		private void Triangulate()
		{
			if (MathUtils.IsClockwise(_originalPolygon))
			{
				throw new ArgumentException($"The provided polygon of length ({_originalPolygon.Count}) was found to have a mostly external area, indicating that it's vertices are not counter-clockwise or that it is self-intersecting. Only simple polygons (non-self-intersecting) can be Ear Clipped, and their vertices should be in counter-clockwise order.");
			}

			if (_originalPolygon.Count() == 3)
			{
				_triangles.AddRange(Enumerable.Range(0, 3));
			}

			_clippedPolygon = new LinkedList<int>(Enumerable.Range(0, _originalPolygon.Count()));

			// Start by determining all convexes, concaves, and ears.
			LinkedListNode<int> currentNode = _clippedPolygon.First;
			while (currentNode != null)
			{
				UpdateNodeStatus(currentNode);
				currentNode = currentNode.Next;
			}

			Clip(_ears.First());

			if (_triangles.Count != _expectedTriCount)
			{
				throw new InvalidOperationException($"After triangulation, polygon with vertex count '{_originalPolygon.Count}' was expected to have {_expectedTriCount} registered triangle verts, but instead had '{_triangles.Count}'. This means the algorithm has not worked.");
			}
		}

		/// <summary> Remove a given ear vertex, and recursively continue to remove ear vertices until triangulation is complete. </summary>
		private void Clip(LinkedListNode<int> earNode)
		{
			if (!_ears.Remove(earNode))
			{
				throw new ArgumentException($"Cannot clip the ear with index '{earNode.Value}' because it was not within the ear set.");
			}

			GetNeighbors(earNode, out LinkedListNode<int> earStartNode, out LinkedListNode<int> earEndNode);

			// Add the triangle in counter-clockwise order.
			_triangles.Add(earStartNode.Value);
			_triangles.Add(earNode.Value);
			_triangles.Add(earEndNode.Value);

			_clippedPolygon.Remove(earNode);
			_remainingVertCount--;


			UpdateNodeStatus(earStartNode);
			UpdateNodeStatus(earEndNode);

			if (_remainingVertCount == 3)
			{
				_triangles.AddRange(_clippedPolygon);
				return;
			}

			Clip(_ears.First());
		}

		/// <summary> Re-evaluate if a node is concave and if it qualifies as an ear vertex. </summary>
		private void UpdateNodeStatus(LinkedListNode<int> vertexNode)
		{
			if (IsConvex(vertexNode))
			{
				_concave.Remove(vertexNode.Value);
				if (TriangleContainsPoints(vertexNode))
				{
					_ears.Remove(vertexNode);
				}
				else
				{
					_ears.Add(vertexNode);
				}
			}
			else
			{
				_concave.Add(vertexNode.Value);
				_ears.Remove(vertexNode);
			}
		}

		private bool IsConvex(LinkedListNode<int> vertexNode)
		{
			GetNeighbors(vertexNode, out LinkedListNode<int> prevNode, out LinkedListNode<int> nextNode);

			Vector2 prev = _originalPolygon[prevNode.Value];
			Vector2 current = _originalPolygon[vertexNode.Value];
			Vector2 next = _originalPolygon[nextNode.Value];

			return MathUtils.Angle(next - current, prev - current) <= 0f;
		}

		/// <summary> Determine if the triangle formed by this node and its two neighbors contains any other vertices. </summary>
		private bool TriangleContainsPoints(LinkedListNode<int> vertexNode)
		{
			GetNeighbors(vertexNode, out LinkedListNode<int> previous, out LinkedListNode<int> next);
			int a = previous.Value;
			int b = vertexNode.Value;
			int c = next.Value;

			foreach (int concave in _concave)
			{
				if (concave == a || concave == b || concave == c)
				{
					continue;
				}
				if (MathUtils.TriangleContainsPoint(_originalPolygon[a], _originalPolygon[b], _originalPolygon[c], _originalPolygon[concave]))
				{
					return true;
				}
			}
			return false;
		}

		private void GetNeighbors(LinkedListNode<int> vertexNode, out LinkedListNode<int> previous, out LinkedListNode<int> next)
		{
			previous = vertexNode.Previous ?? vertexNode.List.Last;
			next = vertexNode.Next ?? vertexNode.List.First;
		}

		/// <summary>
		/// Get the results of this triangulation.
		/// This is an array of indices, with each trio of indices being the counter-clockwise points on the original polygon that define the triangle.
		/// </summary>
		/// <returns></returns>
		public int[] GetTriangles() => _triangles.ToArray();

		/// <summary>
		/// Triangulate the provided <paramref name="polygon"/> using the ear-clipping method.
		/// The polygon must have no holes in it, and must not self-intersect, for the results to be accurate.
		/// </summary>
		public static int[] Triangulate(IList<Vector2> polygon)
		{
			EarClipper clipper = new EarClipper(polygon);
			return clipper.GetTriangles();
		}
	}
}
