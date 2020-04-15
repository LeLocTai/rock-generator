using System;
using MeshDecimator.Math;
using Vector3 = MeshDecimator.Math.Vector3;

namespace RockGen
{
public struct Vector
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double GetMagnitude()
    {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
    }
}

public struct RockGenerationSettings
{
    public VoronoiGridSettings GridSettings { get; set; }

    public Matrix4x4 Transform
    {
        get => transform;
        set
        {
            transform = value;
            Matrix4x4.Invert(transform, out inverseTransform);

            Scale = new Vector(
                Math.Sqrt(transform.M11 * transform.M11 +
                          transform.M12 * transform.M12 +
                          transform.M13 * transform.M13),
                Math.Sqrt(transform.M21 * transform.M21 +
                          transform.M22 * transform.M22 +
                          transform.M23 * transform.M23),
                Math.Sqrt(transform.M31 * transform.M31 +
                          transform.M32 * transform.M32 +
                          transform.M33 * transform.M33)
            );
        }
    }

    public Matrix4x4 InverseTransform
    {
        get => inverseTransform;
        set => inverseTransform = value;
    }

    public Vector Scale { get; private set; }

    public float StockDensity        { get; set; }
    public int   TargetTriangleCount { get; set; }
    public float Distortion          { get; set; }

    Matrix4x4 transform;
    Matrix4x4 inverseTransform;


    public RockGenerationSettings(RockGenerationSettings other) : this()
    {
        Transform           = other.Transform;
        GridSettings        = other.GridSettings;
        Scale               = other.Scale;
        StockDensity        = other.StockDensity;
        TargetTriangleCount = other.TargetTriangleCount;
        Distortion          = other.Distortion;
    }
}
}
