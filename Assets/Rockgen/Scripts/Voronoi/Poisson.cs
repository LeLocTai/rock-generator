//using System.Collections.Generic;
//using UnityEngine;
//
//namespace DelaunayVoronoi
//{
//public class Poisson
//{
//    private const int NUM_ITERATION = 30;
//
//    private readonly Bounds        bounds;
//    private readonly float         radiusSqr;
//    private readonly float         cellSize;
//    private readonly Vector3[,,]   grid;
//    private readonly List<Vector3> activeSamples = new List<Vector3>();
//
//    public Poisson(float size, float radius)
//    {
//        bounds      = new Bounds(Vector3.zero, Vector3.one * size);
//        radiusSqr = radius * radius;
//        cellSize  = radius / Mathf.Sqrt(2);
//        grid = new Vector3[
//            Mathf.CeilToInt(size / cellSize),
//            Mathf.CeilToInt(size / cellSize),
//            Mathf.CeilToInt(size / cellSize)
//        ];
//    }
//
//    public IEnumerable<Vector3> Next()
//    {
//        yield return AddSample(new Vector3(Random.value * bounds.size.x,
//                                           Random.value * bounds.size.y,
//                                           Random.value * bounds.size.z));
//
//        while (activeSamples.Count > 0)
//        {
//            // Pick a random active sample
//            int     i      = (int) Random.value * activeSamples.Count;
//            Vector3 sample = activeSamples[i];
//
//            bool found = false;
//            for (int j = 0; j < NUM_ITERATION; ++j)
//            {
//                float angle = 2 * Mathf.PI * Random.value;
//                // https://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
//                float r = Mathf.Sqrt(Random.value * 3 * radiusSqr + radiusSqr);
//                Vector3 candidate = sample + r * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
//
//                // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
//                if (bounds.Contains(candidate) && IsFarEnough(candidate))
//                {
//                    found = true;
//                    yield return AddSample(candidate);
//                    break;
//                }
//            }
//
//            if (!found)
//            {
//                activeSamples[i] = activeSamples[activeSamples.Count - 1];
//                activeSamples.RemoveAt(activeSamples.Count - 1);
//            }
//        }
//    }
//
//    private bool IsFarEnough(Vector3 sample)
//    {
//        GridPos pos = new GridPos(sample, cellSize);
//
//        int xmin = Mathf.Max(pos.x - 2, 0);
//        int ymin = Mathf.Max(pos.y - 2, 0);
//        int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
//        int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);
//
//        for (int y = ymin; y <= ymax; y++)
//        {
//            for (int x = xmin; x <= xmax; x++)
//            {
//                Vector3 s = grid[x, y];
//                if (s != Vector3.zero)
//                {
//                    Vector3 d = s - sample;
//                    if (d.x * d.x + d.y * d.y < radiusSqr) return false;
//                }
//            }
//        }
//
//        return true;
//
//        // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
//        // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
//        // and we might end up with another sample too close from (0, 0). This is a very minor issue.
//    }
//
//    /// Adds the sample to the active samples queue and the grid before returning it
//    private Vector3 AddSample(Vector3 sample)
//    {
//        activeSamples.Add(sample);
//        GridPos pos = new GridPos(sample, cellSize);
//        grid[pos.x, pos.y] = sample;
//        return sample;
//    }
//
//    /// Helper struct to calculate the x and y indices of a sample in the grid
//    private struct GridPos
//    {
//        public int x;
//        public int y;
//
//        public GridPos(Vector3 sample, float cellSize)
//        {
//            x = (int) (sample.x / cellSize);
//            y = (int) (sample.y / cellSize);
//        }
//    }
//}
//}
