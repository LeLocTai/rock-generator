using System.Linq;
using Rockgen.Primitive;
using UnityEditor;
using UnityEngine;
using UnityMeshSimplifier;
using static UnityEngine.Vector3;

namespace RockGen
{
public class Rock : MonoBehaviour
{
    public float baseResolution = 8;

    [Range(0, 100)]
    public float quality = 50f;

    public float distortion = .5f;
    public float midlevel   = .5f;

    public bool  update;
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
        if (update) return;
        if (!EditorApplication.isPlaying) return;

        ApplyTransformation();
    }

    void Update()
    {
        if (update)
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
                // Debug.DrawRay(worldResult, worldNormal * .2f, Color.green);
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

        mesh.RecalculateNormals();

        // CalcUV(mesh);

        meshFilter.mesh = mesh;
    }

    void CalcUV(Mesh mesh)
    {
        var vertices = mesh.vertices;
        var normals  = mesh.normals;
        var uv       = new Vector2[vertices.Length];

        var center         = transform.position;
        var corners        = new Vector3[8];
        var cornersForward = new Vector3[8];
        grid.GetCellCorners(center, corners);

        for (var i = 0; i < corners.Length; i++)
        {
            cornersForward[i] = (center - corners[i]).normalized;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            var worldPos    = transform.TransformPoint(vertices[i]);
            var worldNormal = transform.TransformDirection(normals[i]);

            var smallestD    = float.PositiveInfinity;
            var nearestIndex = 0;

            for (var j = 0; j < cornersForward.Length; j++)
            {
                float d = Dot(cornersForward[j], worldNormal);

                if (d < smallestD)
                {
                    smallestD    = d;
                    nearestIndex = j;
                }
            }

            var normal    = cornersForward[nearestIndex];
            var projected = ProjectPointOnPlane(worldPos, corners[nearestIndex], normal);

            var tangent  = Cross(normal,  up);
            var binormal = Cross(tangent, normal);
            tangent = Cross(normal, binormal);

            Debug.DrawRay(worldPos, tangent * .1f,  Color.red);
            Debug.DrawRay(worldPos, binormal * .1f, Color.green);

            Debug.DrawLine(worldPos, corners[nearestIndex], Color.gray);

            uv[i] = new Vector2(
                Dot(projected, tangent),
                Dot(projected, binormal)
            );
        }

        mesh.uv = uv;
    }

    static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrig, Vector3 planeNormal)
    {
        var v    = point - planeOrig;
        var dist = Dot(v, planeNormal);
        return point - dist * planeNormal;
    }
}
}
