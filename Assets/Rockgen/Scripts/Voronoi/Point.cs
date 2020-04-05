using System.Collections.Generic;
using UnityEngine;

namespace DelaunayVoronoi
{
public struct Point
{
    public readonly Vector3 coord;

    public HashSet<Tetrahedron> ConnectedSimplexes { get; }

    public Point(Vector3 coord)
    {
        ConnectedSimplexes = new HashSet<Tetrahedron>();

        this.coord = coord;
    }

    public Point(float x, float y, float z = 0)
    {
        ConnectedSimplexes = new HashSet<Tetrahedron>();

        coord = new Vector3(x, y, z);
    }

    public static implicit operator Vector3(Point point)
    {
        return point.coord;
    }

    public static implicit operator Point(Vector3 coord)
    {
        return new Point(coord);
    }

    public bool Equals(Point other)
    {
        return Mathf.Approximately((other.coord - coord).sqrMagnitude, 0);
//        return coord.Equals(other.coord);
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return coord.GetHashCode();
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Point a, Point b)
    {
        return !a.Equals(b);
    }

    public override string ToString()
    {
        return coord.ToString();
    }
}
}
