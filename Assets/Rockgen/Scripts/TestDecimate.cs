using System.Diagnostics;
using UnityEngine;
using UnityMeshSimplifier;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(MeshFilter))]
public class TestDecimate : MonoBehaviour
{
    MeshFilter filter;
    Mesh       orig;
    int        origVertCount;
    int        origTrigCount;

    [Range(0, 100)]
    public float quality = 100;

    public bool   preserveBorderEdges      = false;
    public bool   preserveUVSeamEdges      = false;
    public bool   preserveUVFoldoverEdges  = false;
    public bool   preserveSurfaceCurvature = false;
    public int    maxIterationCount        = 100;
    public double agressiveness            = 7.0;

    void OnEnable()
    {
        filter        = GetComponent<MeshFilter>();
        orig          = filter.sharedMesh;
        origVertCount = orig.vertexCount;
        origTrigCount = orig.triangles.Length / 3;
    }

    [ContextMenu(nameof(Decimate))]
    public void Decimate()
    {
        var sw = Stopwatch.StartNew();

        var simplifier = new MeshSimplifier();
        simplifier.PreserveBorderEdges      = preserveBorderEdges;
        simplifier.PreserveUVSeamEdges      = preserveUVSeamEdges;
        simplifier.PreserveUVFoldoverEdges  = preserveUVFoldoverEdges;
        simplifier.PreserveSurfaceCurvature = preserveSurfaceCurvature;
        simplifier.MaxIterationCount        = maxIterationCount;
        simplifier.Agressiveness            = agressiveness;
        simplifier.Initialize(orig);
        simplifier.SimplifyMesh(quality / 100f);
        filter.sharedMesh = simplifier.ToMesh();
//        filter.sharedMesh.RecalculateNormals();

        sw.Stop();

        Debug.LogFormat("From {0} verts {1} tris to {2} verts {3} tris in {4}ms",
                        origVertCount,
                        origTrigCount,
                        filter.mesh.vertexCount,
                        filter.mesh.triangles.Length / 3,
                        sw.ElapsedMilliseconds);
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            Decimate();
    }
}
