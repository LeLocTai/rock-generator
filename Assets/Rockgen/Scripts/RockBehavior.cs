using System.Linq;
using System.Numerics;
using MeshDecimator;
using MeshDecimator.Algorithms;
using MeshDecimator.Math;
using Rockgen.Primitive;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Vector3;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Mesh = UnityEngine.Mesh;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace RockGen
{
public class RockBehavior : MonoBehaviour
{
    public RockGenerationSettings settings;

    public bool  update;
    public bool  debug;
    public Color debugColor = Color.white;

    RockGenerator generator;
    MeshFilter    meshFilter;

    void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();

        settings.grid      = FindObjectOfType<VoronoiGridBehaviour>().grid;
        settings.size      = ToDV3(transform.lossyScale);
        settings.Transform = ToSMatrix(transform.localToWorldMatrix);

        settings.foundNearest += OnSettingsOnfoundNearest;

        generator = new RockGenerator(settings);
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
        if (update) return;
        if (!EditorApplication.isPlaying) return;
        if (!meshFilter) return;

        meshFilter.mesh = ToUnityMesh(generator.ApplyTransformation());
    }

    void Update()
    {
        if (update)
        {
            settings.Transform = ToSMatrix(transform.localToWorldMatrix);
            meshFilter.mesh    = ToUnityMesh(generator.ApplyTransformation());
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

    static System.Numerics.Matrix4x4 ToSMatrix(Matrix4x4 m)
    {
        return new System.Numerics.Matrix4x4(
            m.m00, m.m01, m.m02, m.m03,
            m.m10, m.m11, m.m12, m.m13,
            m.m20, m.m21, m.m22, m.m23,
            m.m30, m.m31, m.m32, m.m33
        );
    }

    static MeshDecimator.Math.Vector3 ToDV3(Vector3 vec)
    {
        return new MeshDecimator.Math.Vector3(vec.x, vec.y, vec.z);
    }
}
}
