using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
public class DelaunayTriangulator
{
    private float MaxX { get; set; }
    private float MaxY { get; set; }
    private float MaxZ { get; set; }

    public IEnumerable<Tetrahedron> border;

    public List<Point> GeneratePoints(int amount, float maxX, float maxY, float maxZ)
    {
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;

        var point0 = new Point(0,    0,    0);
        var point1 = new Point(maxX, 0,    0);
        var point2 = new Point(0,    maxY, 0);
        var point3 = new Point(0,    0,    maxZ);
        var points = new List<Point> {
            point0,
            point1,
            point2,
            point3,
        };

        border = new List<Tetrahedron> {
            new Tetrahedron(
                point0,
                point1,
                point2,
                point3,
                true
            ),
        };

        var gridSizeX = maxX / amount;
        var gridSizeY = maxY / amount;
        var gridSizeZ = maxZ / amount;

        for (float z = 0; z < maxZ; z += gridSizeZ)
        for (float y = 0; y < maxY; y += gridSizeY)
        for (float x = 0; x < maxX; x += gridSizeX)
        {
            var pointX = x + .5f;// Random.value * gridSizeX;
            var pointY = y + .5f;// Random.value * gridSizeY;
            var pointZ = z + .5f;// Random.value * gridSizeZ;
            points.Add(new Point(pointX, pointY, pointZ));
        }

        return points;
    }

    public IEnumerator BowyerWatson(List<Point> points, HashSet<Tetrahedron> triangulation)
    {
        // must be large enough to completely contain all the points in pointList
        triangulation.Clear();
        triangulation.UnionWith(border);

//        points.AddRange(border.SelectMany(t => t.Vertices));

//        Debug.Log("============== Start ======================================================");

        // add all the points one at a time to the triangulation
        foreach (var point in points)
        {
//            Debug.Log("============== Start Point ==================");
            // first find all the triangles that are no longer valid due to the insertion
            var conflicting = new HashSet<Tetrahedron>(
                triangulation.Where(t => t.CircumSphereContains(point))
            );

//            foreach (var tetrahedron in triangulation)
//            {
//                bool bad = tetrahedron.CircumSphereContains(point);
//                if (bad)
//                    conflicting.Add(tetrahedron);
//            }

            var conflictingFaces = conflicting.SelectMany(t => t.Faces).ToList();
//            Debug.Log("\tConflicting Faces Count:\t" + conflictingFaces.Count);

            var outerConflictingFaces = conflictingFaces
                                       .GroupBy(f => f)
                                       .Where(g => g.Count() == 1)
                                       .Select(g => g.Key)
                                       .ToList();
//            Debug.Log("\tBoundary Faces Count:\t" + outerConflictingFaces.Count);

            // find the boundary of the polygonal hole
            var polygon = new HashSet<Triangle>(outerConflictingFaces);

            if (polygon.Count != outerConflictingFaces.Count)
                Debug.LogError("\tBoundary Faces Set Count:\t" + polygon.Count);


//            foreach (var tetrahedron in conflicting)
//            {
//                for (var i = 0; i < tetrahedron.Faces.Length; i++)
//                {
//                    var face         = tetrahedron.Faces[i];
//                    var isSharedFace = false;
//
//                    foreach (var other in conflicting)
//                    {
//                        if (other == tetrahedron)
//                            continue;
//
//                        if (other.HaveFace(face))
//                        {
//                            isSharedFace = true;
//                            break;
//                        }
//                    }
//
//                    if (!isSharedFace)
//                        polygon.Add(face);
//                }
//            }

            // remove them from the data structure
            foreach (var tetrahedron in conflicting)
            {
                foreach (var vertex in tetrahedron.Faces)
                {
//                    vertex.AdjacentTriangles.Remove(tetrahedron);
                }

                triangulation.Remove(tetrahedron);
            }

            // re-triangulate the polygonal hole
            foreach (var face in polygon)
            {
                var tetra = new Tetrahedron(point, face.Vertices[0], face.Vertices[1], face.Vertices[2]);
                triangulation.Add(tetra);
            }
        }

//        Debug.Log("Total Result Count:\t" + triangulation.Count);
        // done inserting points, now clean up
        triangulation.RemoveWhere(o => o.Vertices.Any(v => border.Any(t => t.Vertices.Contains(v))));

//        Debug.Log("Clean Result Count:\t" + triangulation.Count);

        yield return null;
    }

    static void DrawTetrahedron(Tetrahedron tetrahedron, Color color)
    {
        foreach (var face in tetrahedron.Faces)
        {
            DrawTriangle(face, color);
        }
    }

    static void DrawTriangle(Triangle triangle, Color color)
    {
        foreach (var edge in triangle.Edges)
        {
            DrawEdge(edge, color);
        }
    }

    static void DrawEdge(Edge edge, Color color)
    {
        Debug.DrawLine(edge.Point1, edge.Point2, color);
    }
}
}
