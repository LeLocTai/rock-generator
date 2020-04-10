using System;

namespace RockGen
{
[Serializable]
public class VoronoiGridSettings
{
    public int   size       = 4;
    public float randomness = .75f;

    public bool debug;
}
}
