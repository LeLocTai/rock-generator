using System;
using MeshDecimator.Math;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Vector3 = MeshDecimator.Math.Vector3;

namespace RockGen
{
[Serializable]
public class RockGenerationSettings
{
    public Matrix4x4 Transform
    {
        get => transform;
        set
        {
            transform = value;
            Matrix4x4.Invert(transform, out inverseTransform);
        }
    }

    public VoronoiGrid grid;

    public Matrix4x4 inverseTransform;
    public Vector3   size;
    public float     baseResolution = 8;

    public int targetTriangleCount = 1000;

    public float distortion = .5f;
    public float midlevel   = .5f;

    internal event Action<Vector3d, Vector3d, Vector3d> foundNearest;

    internal void OnFoundNearest(Vector3d vertex, Vector3d normal, Vector3d nearest)
    {
        foundNearest?.Invoke(vertex, normal, nearest);
    }

    Matrix4x4 transform;
}
}
