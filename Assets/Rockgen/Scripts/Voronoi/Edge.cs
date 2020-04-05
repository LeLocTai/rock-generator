namespace DelaunayVoronoi
{
public struct Edge
{
    public Point Point1 { get; }
    public Point Point2 { get; }

    public Edge(Point point1, Point point2)
    {
        Point1 = point1;
        Point2 = point2;
    }

    public bool Equals(Edge other)
    {
        var samePoints         = Point1 == other.Point1 && Point2 == other.Point2;
        var samePointsReversed = Point1 == other.Point2 && Point2 == other.Point1;
        return samePoints || samePointsReversed;
    }

    public override bool Equals(object other)
    {
        return other is Edge edge && Equals(edge);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Point1.GetHashCode();
            hashCode = (hashCode * 397) ^ Point2.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(Edge a, Edge b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Edge a, Edge b)
    {
        return !a.Equals(b);
    }
}
}
