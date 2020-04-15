using System;

namespace RockGen
{
public struct VoronoiGridSettings
{
    public int   Size       { get; set; }
    public float Randomness { get; set; }

    public bool Equals(VoronoiGridSettings other)
    {
        return Size == other.Size && Randomness.Equals(other.Randomness);
    }

    public override bool Equals(object obj)
    {
        return obj is VoronoiGridSettings other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Size * 397) ^ Randomness.GetHashCode();
        }
    }
}
}
