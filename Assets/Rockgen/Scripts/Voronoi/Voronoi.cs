using System.Collections.Generic;

namespace DelaunayVoronoi
{
public class Voronoi
{
    public IEnumerable<Edge> GenerateEdgesFromDelaunay(IEnumerable<Tetrahedron> triangulation)
    {
        var voronoiEdges = new HashSet<Edge>();
        foreach (var simplex in triangulation)
        {
            foreach (var neighbour in simplex.Neighbours)
            {
                var edge = new Edge(simplex.circumCenter, neighbour.circumCenter);
                voronoiEdges.Add(edge);
            }
        }

        return voronoiEdges;
    }
}
}
