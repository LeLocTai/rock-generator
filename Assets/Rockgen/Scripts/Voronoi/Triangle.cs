using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Vector3;

namespace DelaunayVoronoi
{
public struct Triangle
{
    public Point[] Vertices { get; }

    public readonly float radiusSquared;
    public readonly Point circumCenter;

    Edge[] edges;

    public Edge[] Edges
    {
        get
        {
            if (edges == null)
            {
                edges    = new Edge[3];
                edges[0] = new Edge(Vertices[0], Vertices[1]);
                edges[1] = new Edge(Vertices[1], Vertices[2]);
                edges[2] = new Edge(Vertices[2], Vertices[0]);
            }

            return edges;
        }
    }

//    public IEnumerable<Triangle> TrianglesWithSharedEdge
//    {
//        get
//        {
//            var self      = this;
//            var neighbors = new HashSet<Triangle>();
//            foreach (var vertex in Vertices)
//            {
//                var trianglesWithSharedEdge = vertex.ConnectedSimplexes.Where(o => o != self && self.SharesEdgeWith(o));
//                neighbors.UnionWith(trianglesWithSharedEdge);
//            }
//
//            return neighbors;
//        }
//    }

    public bool HaveEdge(Edge edge)
    {
        for (int i = 0; i < 3; i++)
            if (Edges[i] == edge)
                return true;

        return false;
    }

    public Triangle(Point point1, Point point2, Point point3)
    {
        Vertices = new Point[3];
        if (!IsCounterClockwise(point1, point2, point3))
        {
            Vertices[0] = point1;
            Vertices[1] = point3;
            Vertices[2] = point2;
        }
        else
        {
            Vertices[0] = point1;
            Vertices[1] = point2;
            Vertices[2] = point3;
        }

        UpdateCircumCircle(Vertices[0], Vertices[1], Vertices[2],
                           out radiusSquared,
                           out circumCenter);

        edges = null;

//        Vertices[0].ConnectedSimplexes.Add(this);
//        Vertices[1].ConnectedSimplexes.Add(this);
//        Vertices[2].ConnectedSimplexes.Add(this);
    }

    private static void UpdateCircumCircle(Vector3   A, Vector3 B, Vector3 C,
                                           out float radiusSquared,
                                           out Point circumCenter)
    {
        // https://en.wikipedia.org/wiki/Circumscribed_circle
        var a = A - C;
        var b = B - C;

        var aLenSqr = a.sqrMagnitude;
        var bLenSqr = b.sqrMagnitude;

        var aCrossb       = Cross(a, b);
        var aCrossbLenSqr = aCrossb.sqrMagnitude;

        radiusSquared = aLenSqr * bLenSqr * (a - b).sqrMagnitude / (4 * aCrossbLenSqr);
        circumCenter  = Cross(aLenSqr * b - bLenSqr * a, aCrossb) / (2 * aCrossbLenSqr) + C;
    }

    private static bool IsCounterClockwise(Point point1, Point point2, Point point3)
    {
        var a = point2.coord - point1.coord;
        var b = point3.coord - point1.coord;

        var normal = Cross(a, b);
        var view   = a - zero;

        return Dot(normal, view) > 0;
    }

    public bool SharesEdgeWith(Triangle triangle)
    {
        var sharedVertices = Vertices.Count(o => triangle.Vertices.Contains(o));
        return sharedVertices == 2;
    }

    public bool CircumcircleContains(Point point)
    {
        return (point.coord - circumCenter).sqrMagnitude < radiusSquared;
    }

    public bool Equals(Triangle other)
    {
        for (var i = 0; i < 3; i++)
            if (Vertices[i] != other.Vertices[i])
                return false;

        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is Triangle other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Vertices[0].GetHashCode();
            hashCode = (hashCode * 397) ^ Vertices[1].GetHashCode();
            hashCode = (hashCode * 397) ^ Vertices[2].GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(Triangle a, Triangle b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Triangle a, Triangle b)
    {
        return !a.Equals(b);
    }
}
}
