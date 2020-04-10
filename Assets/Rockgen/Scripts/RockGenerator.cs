using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using MeshDecimator;
using MeshDecimator.Algorithms;
using MeshDecimator.Math;
using Rockgen.Primitive;
using Vector3 = MeshDecimator.Math.Vector3;

namespace RockGen
{
public class RockGenerator
{
    public readonly RockGenerationSettings settings;

    readonly Mesh origMesh;

    public RockGenerator(RockGenerationSettings settings)
    {
        this.settings = settings;

        SphereCubeFactory.Instance.Radius = .5f;

        SphereCubeFactory.Instance.NumSubDivX = (int) Math.Round(settings.baseResolution * settings.size.x);
        SphereCubeFactory.Instance.NumSubDivY = (int) Math.Round(settings.baseResolution * settings.size.x);
        SphereCubeFactory.Instance.NumSubDivZ = (int) Math.Round(settings.baseResolution * settings.size.x);

        origMesh = SphereCubeFactory.Instance.Create();
    }

    public Mesh ApplyTransformation()
    {
        var vertices = new Vector3d[origMesh.VertexCount];
        var normals  = new Vector3[origMesh.VertexCount];

        var distort = settings.distortion * (settings.size.x + settings.size.y + settings.size.z) / 3f;

        for (var i = 0; i < vertices.Length; i++)
        {
            var worldPos    = Transform(settings.Transform, origMesh.Vertices[i]);
            var worldNormal = TransformDir(settings.Transform, origMesh.Normals[i]);

            var (nearest, nearestDS) = settings.grid.Nearest(worldPos);

            var worldResult = worldPos + worldNormal * ((nearestDS - settings.midlevel) * distort);

            vertices[i] = Transform(settings.inverseTransform, worldResult);
            normals[i]  = origMesh.Normals[i]; // (Vector3) TransformDir(settings.inverseTransform, worldNormal);

            settings.OnFoundNearest(worldResult, worldNormal, nearest);
        }

        var mesh = new Mesh(
            vertices,
            origMesh.Indices
        );
        mesh.Normals = normals;

        var simplifier = new FastQuadricMeshSimplification();
        simplifier.Initialize(mesh);
        simplifier.DecimateMesh(settings.targetTriangleCount);

        mesh = simplifier.ToMesh();

        mesh.RecalculateNormals();

        // CalcUV(mesh);

        return mesh;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3d Transform(Matrix4x4 m, Vector3d v)
    {
        return new Vector3d(
            m.M11 * v.x + m.M12 * v.y + m.M13 * v.z + m.M14,
            m.M21 * v.x + m.M22 * v.y + m.M23 * v.z + m.M24,
            m.M31 * v.x + m.M32 * v.y + m.M33 * v.z + m.M34
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3d TransformDir(Matrix4x4 m, Vector3d v)
    {
        return new Vector3d(
            m.M11 * v.x + m.M12 * v.y + m.M13 * v.z,
            m.M21 * v.x + m.M22 * v.y + m.M23 * v.z,
            m.M31 * v.x + m.M32 * v.y + m.M33 * v.z
        );
    }

    // void CalcUV(Mesh mesh)
    // {
    //     var vertices = mesh.vertices;
    //     var normals  = mesh.normals;
    //     var uv       = new Vector2[vertices.Length];
    //
    //     var center         = transform.position;
    //     var corners        = new Vector3[8];
    //     var cornersForward = new Vector3[8];
    //     grid.GetCellCorners(center, corners);
    //
    //     for (var i = 0; i < corners.Length; i++)
    //     {
    //         cornersForward[i] = (center - corners[i]).normalized;
    //     }
    //
    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         var worldPos    = transform.TransformPoint(vertices[i]);
    //         var worldNormal = transform.TransformDirection(normals[i]);
    //
    //         var smallestD    = float.PositiveInfinity;
    //         var nearestIndex = 0;
    //
    //         for (var j = 0; j < cornersForward.Length; j++)
    //         {
    //             float d = Dot(cornersForward[j], worldNormal);
    //
    //             if (d < smallestD)
    //             {
    //                 smallestD    = d;
    //                 nearestIndex = j;
    //             }
    //         }
    //
    //         var normal    = cornersForward[nearestIndex];
    //         var projected = ProjectPointOnPlane(worldPos, corners[nearestIndex], normal);
    //
    //         var tangent  = Cross(normal,  up);
    //         var binormal = Cross(tangent, normal);
    //         tangent = Cross(normal, binormal);
    //
    //         Debug.DrawRay(worldPos, tangent * .1f,  Color.red);
    //         Debug.DrawRay(worldPos, binormal * .1f, Color.green);
    //
    //         Debug.DrawLine(worldPos, corners[nearestIndex], Color.gray);
    //
    //         uv[i] = new Vector2(
    //             Dot(projected, tangent),
    //             Dot(projected, binormal)
    //         );
    //     }
    //
    //     mesh.uv = uv;
    // }
    //
    // static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrig, Vector3 planeNormal)
    // {
    //     var v    = point - planeOrig;
    //     var dist = Dot(v, planeNormal);
    //     return point - dist * planeNormal;
    // }
}
}
