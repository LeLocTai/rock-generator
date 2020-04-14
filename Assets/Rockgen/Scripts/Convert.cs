using System.Linq;
using UnityEngine;

namespace RockGenUnity
{
public static class Convert
{
    public static RockGen.Matrix4x4 ToRMatrix(Matrix4x4 m)
    {
        return new RockGen.Matrix4x4(
            m.m00, m.m01, m.m02, m.m03,
            m.m10, m.m11, m.m12, m.m13,
            m.m20, m.m21, m.m22, m.m23,
            m.m30, m.m31, m.m32, m.m33
        );
    }

    public static Mesh ToUnityMesh(MeshDecimator.Mesh dmesh)
    {
        return new Mesh {
            vertices = dmesh.Vertices
                            .Select(v => new Vector3((float) v.x,
                                                     (float) v.y,
                                                     (float) v.z))
                            .ToArray(),
            // uv        = dmesh.UV1.Select(uv => new Vector2(uv.x, uv.y)).ToArray(),
            triangles = dmesh.GetIndices(0),
            normals = dmesh.Normals.Select(v => new Vector3(v.x, v.y, v.z))
                           .ToArray()
        };
    }
}
}
