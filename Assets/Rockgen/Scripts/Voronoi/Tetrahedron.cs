using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Vector3;

namespace DelaunayVoronoi
{
public struct Tetrahedron
{
    public Point[] Vertices { get; }

    public readonly bool  isSupreme;
    public readonly float radiusSquared;
    public readonly Point circumCenter;

    Triangle[] faces;

    public Triangle[] Faces
    {
        get
        {
            return faces ?? (faces = new[] {
                                    new Triangle(Vertices[0], Vertices[1], Vertices[2]),
                                    new Triangle(Vertices[0], Vertices[1], Vertices[3]),
                                    new Triangle(Vertices[1], Vertices[2], Vertices[3]),
                                    new Triangle(Vertices[2], Vertices[0], Vertices[3]),
                                });
        }
    }

    public bool HaveFace(Triangle face)
    {
        for (var i = 0; i < Faces.Length; i++)
            if (Faces[i] == face)
                return true;

        return false;
    }

    public IEnumerable<Tetrahedron> Neighbours
    {
        get
        {
            var self       = this;
            var neighbours = new HashSet<Tetrahedron>();
            foreach (var vertex in Vertices)
            {
                var neighbour = vertex.ConnectedSimplexes
                                      .Where(o => o != self && self.SharesFaceWith(o));
                neighbours.UnionWith(neighbour);
            }

            return neighbours;
        }
    }

//    public Vector3 barty

    public bool SharesFaceWith(Tetrahedron other)
    {
        return Vertices.Intersect(other.Vertices).Count() == 3;
    }

    public Tetrahedron(Point point0, Point point1, Point point2, Point point3, bool isSupreme = false)
    {
        this.isSupreme = isSupreme;

        Vertices    = new Point[4];
        Vertices[0] = point0;
        Vertices[1] = point1;
        Vertices[2] = point2;
        Vertices[3] = point3;

        UpdateCircumSphere(Vertices[0], Vertices[1], Vertices[2], Vertices[3],
                           out radiusSquared,
                           out circumCenter);

        faces = null;

        Vertices[0].ConnectedSimplexes.Add(this);
        Vertices[1].ConnectedSimplexes.Add(this);
        Vertices[2].ConnectedSimplexes.Add(this);
        Vertices[3].ConnectedSimplexes.Add(this);
    }

    private static void UpdateCircumSphere(Vector3   p0, Vector3 p1, Vector3 p2, Vector3 p3,
                                           out float radiusSquared,
                                           out Point circumCenter)
    {
        // https://en.wikipedia.org/wiki/Tetrahedron#Circumradius
        // https://math.stackexchange.com/questions/2414640/circumsphere-of-a-tetrahedron
        var d1       = p1 - p0;
        var d2       = p2 - p0;
        var d3       = p3 - p0;
        var d1LenSqr = d1.sqrMagnitude;
        var d2LenSqr = d2.sqrMagnitude;
        var d3LenSqr = d3.sqrMagnitude;

        var cross23 = Cross(d2, d3);

        var denominator = Dot(2 * d1, cross23);
        var offset = denominator == 0 // co-planar
                         ? d1
                         : (cross23 * d1LenSqr + Cross(d3, d1) * d2LenSqr + Cross(d1, d2) * d3LenSqr) / denominator;

        circumCenter = offset + p0;

        radiusSquared = (circumCenter - p0).sqrMagnitude;
    }

    public bool CircumSphereContains(Point point)
    {
        if (isSupreme)
            return true;

        return (point.coord - circumCenter).sqrMagnitude < radiusSquared;
    }

    public bool Equals(Tetrahedron other)
    {
        for (var i = 0; i < 4; i++)
            if (Vertices[i].coord != other.Vertices[i].coord)
                return false;

        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is Tetrahedron other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Vertices[0].GetHashCode();
            hashCode = (hashCode * 397) ^ Vertices[1].GetHashCode();
            hashCode = (hashCode * 397) ^ Vertices[2].GetHashCode();
            hashCode = (hashCode * 397) ^ Vertices[3].GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(Tetrahedron a, Tetrahedron b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Tetrahedron a, Tetrahedron b)
    {
        return !a.Equals(b);
    }
}
}
