using System;
using MeshDecimator.Math;
using Vector3 = MeshDecimator.Math.Vector3;

namespace RockGen
{
public struct RockGenerationSettings
{
    public VoronoiGridSettings GridSettings { get; set; }

    public Matrix4x4 Transform
    {
        get => transform;
        set
        {
            transform = value;

            Scale = new Vector(Math.Sqrt(transform.M11 * transform.M11 +
                                         transform.M21 * transform.M21 +
                                         transform.M31 * transform.M31),
                               Math.Sqrt(transform.M12 * transform.M12 +
                                         transform.M22 * transform.M22 +
                                         transform.M32 * transform.M32),
                               Math.Sqrt(transform.M13 * transform.M13 +
                                         transform.M23 * transform.M23 +
                                         transform.M33 * transform.M33));
        }
    }

    public Vector Scale { get; private set; }

    public float StockDensity        { get; set; }
    public int   TargetTriangleCount { get; set; }
    public float Distortion          { get; set; }
    public float PatternSize         { get; set; }

    Matrix4x4 transform;

    public RockGenerationSettings(RockGenerationSettings other) : this()
    {
        Transform           = other.Transform;
        GridSettings        = other.GridSettings;
        Scale               = other.Scale;
        StockDensity        = other.StockDensity;
        TargetTriangleCount = other.TargetTriangleCount;
        Distortion          = other.Distortion;
        PatternSize         = other.PatternSize;
    }
}
}
