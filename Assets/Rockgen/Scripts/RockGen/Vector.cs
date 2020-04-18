using System;

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
}
