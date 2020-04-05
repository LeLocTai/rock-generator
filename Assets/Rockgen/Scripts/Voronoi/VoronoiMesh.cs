using System;
using System.Collections.Generic;
using UnityEngine;

namespace DelaunayVoronoi
{
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class VoronoiMesh : MonoBehaviour
{
    MeshFilter meshFilter;

    DelaunayTriangulator triangulator;
    Voronoi              voronoi;

    [SerializeField] List<Point>          points;
    [SerializeField] HashSet<Tetrahedron> dSimplexes;
    [SerializeField] IEnumerable<Edge>    vTriangles;

    public int numPoints = 128;

    void OnEnable()
    {
        meshFilter            = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();

        triangulator = new DelaunayTriangulator();
        voronoi      = new Voronoi();

        Gen();
    }

    void OnDrawGizmos()
    {
        if (points == null) return;

        Gizmos.color = Color.gray;
        foreach (var tetrahedron in triangulator.border)
        {
            foreach (var face in tetrahedron.Faces)
            {
                foreach (var edge in face.Edges)
                {
                    Gizmos.DrawLine(edge.Point1, edge.Point2);
                }
            }
        }


        foreach (var tetrahedron in dSimplexes)
        {
            Gizmos.color = new Color(1, 1, 1, .5f);
            foreach (var face in tetrahedron.Faces)
            {
                foreach (var edge in face.Edges)
                {
                    Gizmos.DrawLine(edge.Point1, edge.Point2);
                }
            }

            Gizmos.color = new Color(1, 0, 0, .05f);
            Gizmos.DrawSphere(tetrahedron.circumCenter, Mathf.Sqrt(tetrahedron.radiusSquared));
        }

        Gizmos.color = Color.blue;
        foreach (var tetrahedron in dSimplexes)
        {
            Gizmos.DrawCube(tetrahedron.circumCenter, Vector3.one * .1f);
        }

        Gizmos.color = new Color(0, 1, .5f, .5f);
        foreach (var edge in vTriangles)
        {
            Gizmos.DrawLine(edge.Point1, edge.Point2);
        }

        Gizmos.color = Color.black;
        foreach (var point in points)
        {
            Gizmos.DrawCube(point, Vector3.one * .1f);
        }
    }

    [ContextMenu("Gen")]
    void Gen()
    {
        points = triangulator.GeneratePoints(numPoints, 10, 10, 10);

        dSimplexes = new HashSet<Tetrahedron>();
        StartCoroutine(triangulator.BowyerWatson(points, dSimplexes));
        vTriangles = voronoi.GenerateEdgesFromDelaunay(dSimplexes);

//        var vertices = new List<Vector3>();
//        var indices  = new List<int>();
//        int id       = 0;
//        foreach (var edge in vTriangles)
//        {
//            vertices.Add(edge.Point1);
//            vertices.Add(edge.Point2);
//            indices.Add(id++);
//            indices.Add(id++);
//        }
//
//        meshFilter.sharedMesh.Clear();
//        meshFilter.sharedMesh.SetVertices(vertices);
//        meshFilter.sharedMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
    }
}
}
