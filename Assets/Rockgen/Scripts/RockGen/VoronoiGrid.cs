using System;
using MeshDecimator.Math;
using static System.Math;

namespace RockGen
{
public class VoronoiGrid
{
    readonly VoronoiGridSettings settings;
    readonly Random              rnd;

    public readonly Vector3d[,,] points;

    public VoronoiGrid(VoronoiGridSettings settings)
    {
        this.settings = settings;
        rnd           = new Random();

        points = new Vector3d[settings.Size, settings.Size, settings.Size];

        for (var z = 0; z < settings.Size; z++)
        for (var y = 0; y < settings.Size; y++)
        for (var x = 0; x < settings.Size; x++)
        {
            var cellCenter = new Vector3(x, y, z);
            points[x, y, z] = cellCenter + RandomDir() * (settings.Randomness / 2f);
        }
    }

    (int, int, int) WorldToGridCoord(Vector3d worldPos)
    {
        return (Clamp((int) Floor(worldPos.x), 0, settings.Size - 1),
                Clamp((int) Floor(worldPos.y), 0, settings.Size - 1),
                Clamp((int) Floor(worldPos.z), 0, settings.Size - 1)
               );
    }

    static int Clamp(int val, int min, int max)
    {
        return Min(Max(val, min), max);
    }

    public (Vector3d, double) Nearest(Vector3d target)
    {
        var nearest  = points[0, 0, 0];
        var nearestD = double.PositiveInfinity;

        var (cellX, cellY, cellZ) = WorldToGridCoord(target);

        int startZ = Max(0, cellZ - 2);
        int startY = Max(0, cellY - 2);
        int startX = Max(0, cellX - 2);

        int endZ = Min(settings.Size, cellZ + 2);
        int endY = Min(settings.Size, cellY + 2);
        int endX = Min(settings.Size, cellX + 2);

        for (int z = startZ; z < endZ; z++)
        for (int y = startY; y < endY; y++)
        for (int x = startX; x < endX; x++)
        {
            var d = (points[x, y, z] - target).MagnitudeSqr;
            if (d < nearestD)
            {
                nearest  = points[x, y, z];
                nearestD = d;
            }
        }

        return (nearest, (nearestD));
    }

    // public void GetCellCorners(Vector3 position, Vector3[] corners)
    // {
    //     if (corners.Length != 8) throw new ArgumentException(nameof(corners) + " should be length 8");
    //
    //     var (x, y, z) = WorldToGridCoord(position);
    //
    //     corners[0] = points[x, y, z];
    //     corners[1] = points[x + 1, y, z];
    //     corners[2] = points[x, y + 1, z];
    //     corners[3] = points[x, y, z + 1];
    //     corners[4] = points[x + 1, y + 1, z];
    //     corners[5] = points[x, y + 1, z + 1];
    //     corners[6] = points[x + 1, y, z + 1];
    //     corners[7] = points[x + 1, y + 1, z + 1];
    // }

    Vector3d RandomDir()
    {
        // http://stackoverflow.com/questions/5408276/python-uniform-spherical-distribution
        var phi      = rnd.NextDouble() * 2d * PI;
        var cosTheta = rnd.NextDouble() * 2d - 1d;
        var sinTheta = Sqrt(1 - cosTheta * cosTheta);
        var r        = Pow(rnd.NextDouble(), 1 / 3d);

        var rSinTheta = r * sinTheta;

        return new Vector3d(
            rSinTheta * Cos(phi),
            rSinTheta * Sin(phi),
            r * cosTheta
        );
    }
}
}
