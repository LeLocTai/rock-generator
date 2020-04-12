using System.Linq;
using MeshDecimator.Math;
using RockGen;
using UnityEditor;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Mesh = UnityEngine.Mesh;
using Vector3 = UnityEngine.Vector3;

namespace RockGenUnity
{
public class RockBehavior : MonoBehaviour
{
    [Header("Grid")]
    public int size = 6;

    public float randomness = .75f;

    [Header("Rock")]
    public float stockDensity = 8;

    public int   targetTriangleCount = 1000;
    public float distortion          = 1;

    [Header("Debug")]
    public bool update;

    public bool  debug;
    public Color debugColor = Color.white;

    RockGenerationSettings settings;
    RockGenerator          generator;
    MeshFilter             meshFilter;

    void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();

        generator = new RockGenerator();

        settings           = new RockGenerationSettings();
        settings.Transform = ToRMatrix(transform.localToWorldMatrix);

        generator.foundNearest += OnSettingsOnfoundNearest;

        ApplySettings();
    }

    void OnSettingsOnfoundNearest(Vector3d vertex, Vector3d normal, Vector3d nearest)
    {
        if (debug)
        {
            var vert = new Vector3((float) vertex.x,  (float) vertex.y,  (float) vertex.z);
            var norm = new Vector3((float) normal.x,  (float) normal.y,  (float) normal.z);
            var near = new Vector3((float) nearest.x, (float) nearest.y, (float) nearest.z);
            Debug.DrawLine(vert, near, debugColor);
            Debug.DrawRay(vert, norm * .2f, Color.green);
        }
    }

    void OnValidate()
    {
        ApplySettings();

        if (update) return;
        if (!EditorApplication.isPlaying) return;
        if (!meshFilter) return;

        meshFilter.mesh = ToUnityMesh(generator.MakeRock());
    }

    void ApplySettings()
    {
        var newGridSettings = new VoronoiGridSettings {
            Size       = size,
            Randomness = randomness
        };

        var newSettings = new RockGenerationSettings(settings) {
            GridSettings        = newGridSettings,
            StockDensity        = stockDensity,
            TargetTriangleCount = targetTriangleCount,
            Distortion          = distortion
        };

        generator.Settings = newSettings;
    }

    void Update()
    {
        if (update)
        {
            settings.Transform = ToRMatrix(transform.localToWorldMatrix);
            ApplySettings();

            meshFilter.mesh = ToUnityMesh(generator.MakeRock());
        }
    }

    static Mesh ToUnityMesh(MeshDecimator.Mesh dmesh)
    {
        return new Mesh {
            vertices = dmesh.Vertices
                            .Select(v => new Vector3((float) v.x,
                                                     (float) v.y,
                                                     (float) v.z))
                            .ToArray(),
            // uv        = dmesh.UV1.Select(uv => new Vector2(uv.x, uv.y)).ToArray(),
            triangles = dmesh.GetIndices(0),
            normals = dmesh.Normals.Select(v => new Vector3(v.x, v.y, v.z))
                           .ToArray()
        };
    }

    static RockGen.Matrix4x4 ToRMatrix(Matrix4x4 m)
    {
        return new RockGen.Matrix4x4(
            m.m00, m.m01, m.m02, m.m03,
            m.m10, m.m11, m.m12, m.m13,
            m.m20, m.m21, m.m22, m.m23,
            m.m30, m.m31, m.m32, m.m33
        );
    }

    void OnDrawGizmos()
    {
        if (!debug) return;

        foreach (var point in generator.Grid.points)
        {
            Gizmos.DrawSphere(ToUV3(point), .03f);
        }

        for (var z = 1; z < generator.Settings.GridSettings.Size; z++)
        for (var y = 1; y < generator.Settings.GridSettings.Size; y++)
        for (var x = 1; x < generator.Settings.GridSettings.Size; x++)
        {
            Gizmos.DrawLine(ToUV3(generator.Grid.points[x - 1, y, z]), ToUV3(generator.Grid.points[x, y, z]));
            Gizmos.DrawLine(ToUV3(generator.Grid.points[x, y - 1, z]), ToUV3(generator.Grid.points[x, y, z]));
            Gizmos.DrawLine(ToUV3(generator.Grid.points[x, y, z - 1]), ToUV3(generator.Grid.points[x, y, z]));
        }
    }

    Vector3 ToUV3(Vector3d v)
    {
        return new Vector3((float) v.x, (float) v.y, (float) v.z);
    }
}
}
