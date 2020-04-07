using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Mathf;
using static UnityEngine.Vector3;
using Random = UnityEngine.Random;

namespace RockGen
{
public class VoronoiGrid : MonoBehaviour
{
    public int   size       = 4;
    public float randomness = .75f;

    public bool debug;

    Vector3[,,] points;

    void OnEnable()
    {
        points = new Vector3[size, size, size];

        for (var z = 0; z < size; z++)
        for (var y = 0; y < size; y++)
        for (var x = 0; x < size; x++)
        {
            var cellCenter = new Vector3(x, y, z);
            points[x, y, z] = cellCenter + Random.insideUnitSphere * (randomness / 2f);
        }
    }

    (int, int, int) WorldToGridCoord(Vector3 worldPos)
    {
        return (Clamp(FloorToInt(worldPos.x), 0, size - 1),
                Clamp(FloorToInt(worldPos.y), 0, size - 1),
                Clamp(FloorToInt(worldPos.z), 0, size - 1)
               );
    }

    public (Vector3, float) Nearest(Vector3 target)
    {
        var nearest  = points[0, 0, 0];
        var nearestD = Infinity;

        // int cellZ = RoundToInt(target.z % size);
        // int cellY = RoundToInt(target.y % size);
        // int cellX = RoundToInt(target.x % size);

        var (cellX, cellY, cellZ) = WorldToGridCoord(target);

        int startZ = Max(0, cellZ - 2);
        int startY = Max(0, cellY - 2);
        int startX = Max(0, cellX - 2);

        int endZ = Min(size, cellZ + 2);
        int endY = Min(size, cellY + 2);
        int endX = Min(size, cellX + 2);

        for (int z = startZ; z < endZ; z++)
        for (int y = startY; y < endY; y++)
        for (int x = startX; x < endX; x++)
        {
            var d = SqrMagnitude(points[x, y, z] - target);
            if (d < nearestD)
            {
                nearest  = points[x, y, z];
                nearestD = d;
            }
        }

        return (nearest, (nearestD));
    }

    public void GetCellCorners(Vector3 position, Vector3[] corners)
    {
        if (corners.Length != 8) throw new ArgumentException(nameof(corners) + " should be length 8");

        var (x, y, z) = WorldToGridCoord(position);

        corners[0] = points[x, y, z];
        corners[1] = points[x + 1, y, z];
        corners[2] = points[x, y + 1, z];
        corners[3] = points[x, y, z + 1];
        corners[4] = points[x + 1, y + 1, z];
        corners[5] = points[x, y + 1, z + 1];
        corners[6] = points[x + 1, y, z + 1];
        corners[7] = points[x + 1, y + 1, z + 1];
    }

    void OnDrawGizmos()
    {
        if (!debug) return;

        if (points == null) return;
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, .03f);
        }

        for (var z = 1; z < size; z++)
        for (var y = 1; y < size; y++)
        for (var x = 1; x < size; x++)
        {
            Gizmos.DrawLine(points[x - 1, y, z], points[x, y, z]);
            Gizmos.DrawLine(points[x, y - 1, z], points[x, y, z]);
            Gizmos.DrawLine(points[x, y, z - 1], points[x, y, z]);
        }
    }
}
}
