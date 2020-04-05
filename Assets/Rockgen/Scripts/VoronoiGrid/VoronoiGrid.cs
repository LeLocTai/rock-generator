using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Vector3;
using Random = UnityEngine.Random;

namespace RockGen
{
class LengthComparer : IComparer<Vector3>
{
    public int Compare(Vector3 x, Vector3 y)
    {
        return x.sqrMagnitude.CompareTo(y.sqrMagnitude);
    }
}

class Tetrahedron
{
    public readonly Vector3[]     vertices;
    public readonly Edge[]        edges;
    public readonly Triangle[]    faces;
    public readonly Tetrahedron[] neighbours;
    public readonly Vector3       circumCenter;

    public int neighboursCount;

    static readonly LengthComparer LENGTH_COMPARER = new LengthComparer();

    public Tetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        vertices = new[] {a, b, c, d};

        edges = new[] {
            new Edge(a, b),
            new Edge(a, c),
            new Edge(a, d),
            new Edge(b, c),
            new Edge(b, d),
            new Edge(c, d)
        };

        faces = new[] {
            new Triangle(a, b, c),
            new Triangle(a, b, d),
            new Triangle(b, c, d),
            new Triangle(c, a, d),
        };

        neighbours      = new Tetrahedron[8];
        neighboursCount = 0;
        circumCenter    = GetCircumCenter(vertices);
    }

    public static Vector3 GetCircumCenter(Vector3[] vertices)
    {
        // https://en.wikipedia.org/wiki/Tetrahedron#Circumradius
        // https://math.stackexchange.com/questions/2414640/circumsphere-of-a-tetrahedron
        var d1       = vertices[1] - vertices[0];
        var d2       = vertices[2] - vertices[0];
        var d3       = vertices[3] - vertices[0];
        var d1LenSqr = d1.sqrMagnitude;
        var d2LenSqr = d2.sqrMagnitude;
        var d3LenSqr = d3.sqrMagnitude;

        var cross23 = Cross(d2, d3);

        var denominator = Dot(2 * d1, cross23);
        var offset = denominator == 0 // co-planar
                         ? d1
                         : (cross23 * d1LenSqr + Cross(d3, d1) * d2LenSqr + Cross(d1, d2) * d3LenSqr) / denominator;

        return offset + vertices[0];
    }

    public bool DoShareFace(Tetrahedron other)
    {
        foreach (var face in faces)
        {
            foreach (var otherFace in other.faces)
            {
                if (face.IsCoPlanar(otherFace)) return true;
            }
        }

        return false;
    }
}

class Edge
{
    public readonly Vector3 a;
    public readonly Vector3 b;

    public Edge(Vector3 a, Vector3 b)
    {
        this.a = a;
        this.b = b;
    }
}

class Triangle
{
    public Vector3[] vertices;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        vertices = new[] {a, b, c};
    }

    public bool IsCoPlanar(Triangle other)
    {
        var s1  = vertices[1] - vertices[0];
        var s2  = vertices[2] - vertices[0];
        var n1  = Cross(s1, s2);
        var err = Abs(Dot(n1, other.vertices[0]));
        if (err >= 1e-3f) return false;

        var o1 = other.vertices[1] - other.vertices[0];
        var o2 = other.vertices[2] - other.vertices[0];
        var n2 = Cross(o1, o2);
        return Abs(Dot(n1, n2)) >= 1 - 1e-3f;
    }
}

public class VoronoiGrid : MonoBehaviour
{
    public int   size       = 4;
    public float randomness = .75f;
    public float dMax       = 2f;
    public float dMin       = 1f;

    public bool debug;

    Vector3[,,] points;

    readonly List<Tetrahedron> tetrahedrons = new List<Tetrahedron>();
    readonly List<Vector3>     corners      = new List<Vector3>();
    readonly HashSet<Edge>     edges        = new HashSet<Edge>();

    void OnEnable()
    {
        points = new Vector3[size, size, size];

        for (var z = 0; z < size; z++)
        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
        {
            var cellCenter = new Vector3(x, y, z);
            points[x, y, z] = cellCenter + Random.insideUnitSphere * (randomness / 2f);
        }

        //
        // tetrahedrons.Clear();
        // corners.Clear();
        //
        // for (var z = 0; z < size - 1; z++)
        // for (var y = 0; y < size - 1; y++)
        // for (var x = 0; x < size - 1; x++)
        // {
        //     var t0 = new Tetrahedron(
        //         points[x, y, z],
        //         points[x + 1, y, z],
        //         points[x, y + 1, z],
        //         points[x, y, z + 1]
        //     );
        //     var t1 = new Tetrahedron(
        //         points[x + 1, y, z],
        //         points[x, y + 1, z],
        //         points[x + 1, y + 1, z],
        //         points[x + 1, y + 1, z + 1]
        //     );
        //     var t2 = new Tetrahedron(
        //         points[x, y + 1, z],
        //         points[x, y, z + 1],
        //         points[x, y + 1, z + 1],
        //         points[x + 1, y + 1, z + 1]
        //     );
        //     var t3 = new Tetrahedron(
        //         points[x + 1, y, z],
        //         points[x, y, z + 1],
        //         points[x + 1, y, z + 1],
        //         points[x + 1, y + 1, z + 1]
        //     );
        //     var t4 = new Tetrahedron(
        //         points[x + 1, y, z],
        //         points[x, y + 1, z],
        //         points[x, y, z + 1],
        //         points[x + 1, y + 1, z + 1]
        //     );
        //
        //     tetrahedrons.Add(t0);
        //     tetrahedrons.Add(t1);
        //     tetrahedrons.Add(t2);
        //     tetrahedrons.Add(t3);
        //     tetrahedrons.Add(t4);
        //     corners.Add(t0.circumCenter);
        //     corners.Add(t1.circumCenter);
        //     corners.Add(t2.circumCenter);
        //     corners.Add(t3.circumCenter);
        //     corners.Add(t4.circumCenter);
        // }
        //
        // foreach (var tetrahedron in tetrahedrons)
        // {
        //     var neighbours = tetrahedrons.Where(t => !t.Equals(tetrahedron) &&
        //                                              t.DoShareFace(tetrahedron));
        //
        //     int currentNeighbourCount = 0;
        //     foreach (var neighbour in neighbours)
        //     {
        //         tetrahedron.neighbours[currentNeighbourCount++] = neighbour;
        //     }
        //
        //
        //     tetrahedron.neighboursCount = currentNeighbourCount;
        // }
        //
        // edges.Clear();
        // foreach (var tetrahedron in tetrahedrons)
        // {
        //     for (var i = 0; i < tetrahedron.neighboursCount; i++)
        //     {
        //         var neighbour = tetrahedron.neighbours[i];
        //         edges.Add(new Edge(tetrahedron.circumCenter, neighbour.circumCenter));
        //     }
        // }
    }

    public (Vector3, float) Nearest(Vector3 target)
    {
        var nearest  = points[0, 0, 0];
        var nearestD = Infinity;

        int cellZ = RoundToInt(target.z % size);
        int cellY = RoundToInt(target.y % size);
        int cellX = RoundToInt(target.x % size);

        int startZ = Max(0, cellZ - 2);
        int startY = Max(0, cellY - 2);
        int startX = Max(0, cellX - 2);

        int endZ = Min(size, cellZ + 2);
        int endY = Min(size, cellY + 2);
        int endX = Min(size, cellX + 2);

        for (int z = startZ; z < endZ; z++)
        for (int y = startY; y < endY; y++)
        for (int x = startX; x < endX; x++)
        {
            var d = SqrMagnitude(points[x, y, z] - target);
            if (d < nearestD)
            {
                nearest  = points[x, y, z];
                nearestD = d;
            }
        }

        return (nearest, (nearestD));
    }

    void OnDrawGizmos()
    {
        if (!debug) return;

        if (points == null) return;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, .03f);
        }

        Vector3 center = one * ((size - 1) / 2f);

        foreach (var corner in corners)
        {
            var dist = Distance(center, corner);
            if (dist >= dMin && dist <= dMax)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawSphere(corner, .05f);
        }

        Gizmos.color = Color.white;
        foreach (var edge in tetrahedrons.SelectMany(t => t.edges))
        {
            Gizmos.DrawLine(edge.a, edge.b);
        }

        foreach (var edge in edges)
        {
            var distA = Distance(center, edge.a);
            var distB = Distance(center, edge.b);
            if (distA >= dMin && distA <= dMax &&
                distB >= dMin && distB <= dMax)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;

            Gizmos.DrawLine(edge.a, edge.b);
        }
    }
}
}
