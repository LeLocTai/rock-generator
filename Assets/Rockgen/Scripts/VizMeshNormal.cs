using System;
using UnityEngine;

public class VizMeshNormal : MonoBehaviour
{
    MeshFilter mf;

    void OnEnable()
    {
        mf = GetComponent<MeshFilter>();
    }

    void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        if (!mf) return;

        var m = mf.mesh;
        var v = m.vertices;
        var n = m.normals;

        for (var i = 0; i < v.Length; i++)
        {
            Gizmos.DrawRay(transform.TransformPoint(v[i]),
                           transform.TransformDirection(n[i]) * .2f);
        }
    }
}
