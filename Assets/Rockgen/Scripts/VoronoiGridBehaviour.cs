using UnityEngine;

namespace RockGen
{
public class VoronoiGridBehaviour : MonoBehaviour
{
    public   VoronoiGridSettings settings;
    internal VoronoiGrid         grid;

    void OnEnable()
    {
        grid = new VoronoiGrid(settings);
    }

    void OnDrawGizmos()
    {
        if (!settings.debug) return;
        if (grid == null) return;

        foreach (var point in grid.points)
        {
            Gizmos.DrawSphere(ToUV3(point), .03f);
        }

        for (var z = 1; z < settings.size; z++)
        for (var y = 1; y < settings.size; y++)
        for (var x = 1; x < settings.size; x++)
        {
            Gizmos.DrawLine(ToUV3(grid.points[x - 1, y, z]), ToUV3(grid.points[x, y, z]));
            Gizmos.DrawLine(ToUV3(grid.points[x, y - 1, z]), ToUV3(grid.points[x, y, z]));
            Gizmos.DrawLine(ToUV3(grid.points[x, y, z - 1]), ToUV3(grid.points[x, y, z]));
        }
    }

    Vector3 ToUV3(MeshDecimator.Math.Vector3d v)
    {
        return new Vector3((float) v.x, (float) v.y, (float) v.z);
    }
}
}
