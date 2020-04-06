using Rockgen.Primitive;
using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;

namespace RockGen
{
public class Rock : MonoBehaviour
{
    public float baseResolution = 8;

    [Range(0, 100)]
    public float quality = 50f;

    public float distortion = .5f;
    public float midlevel   = .5f;

    public bool  debug;
    public Color debugColor = Color.white;

    Mesh        origMesh;
    MeshFilter  meshFilter;
    VoronoiGrid grid;

    void OnEnable()
    {
        grid       = FindObjectOfType<VoronoiGrid>();
        meshFilter = GetComponent<MeshFilter>();

        SphereCubeFactory.Instance.Radius = .5f;

        var scale = transform.lossyScale;
        SphereCubeFactory.Instance.NumSubDivX = Mathf.RoundToInt(baseResolution * scale.x);
        SphereCubeFactory.Instance.NumSubDivY = Mathf.RoundToInt(baseResolution * scale.y);
        SphereCubeFactory.Instance.NumSubDivZ = Mathf.RoundToInt(baseResolution * scale.z);

        origMesh = SphereCubeFactory.Instance.Create();

        ApplyTransformation();
    }

    void OnValidate()
    {
        if (!EditorApplication.isPlaying) return;

        ApplyTransformation();
    }

    void ApplyTransformation()
    {
        var vertices = origMesh.vertices;
        var normals  = origMesh.normals;

        var scale   = transform.lossyScale;
        var distort = distortion * (scale.x + scale.y + scale.z) / 3f;

        for (var i = 0; i < vertices.Length; i++)
        {
            var worldPos    = transform.TransformPoint(vertices[i]);
            var worldNormal = transform.TransformDirection(normals[i]);
            var (nearest, nearestDS) = grid.Nearest(worldPos);

            var worldResult = worldPos + worldNormal * ((nearestDS - midlevel) * distort);

            vertices[i] = transform.InverseTransformPoint(worldResult);
            // normals[i] = transform.InverseTransformDirection(nearest - worldPos).normalized;

            if (debug)
            {
                Debug.DrawLine(worldResult, nearest, debugColor);
                Debug.DrawRay(worldResult, worldNormal * .2f, Color.green);
            }
        }

        var mesh = new Mesh {
            vertices  = vertices,
            uv        = origMesh.uv,
            triangles = origMesh.triangles,
            normals   = normals
        };

        var simplifier = new MeshSimplifier();
        simplifier.Initialize(mesh);
        simplifier.SimplifyMesh(quality / 100f);

        mesh = simplifier.ToMesh();

        meshFilter.mesh = mesh;

        meshFilter.mesh.RecalculateNormals();
    }
}
}
