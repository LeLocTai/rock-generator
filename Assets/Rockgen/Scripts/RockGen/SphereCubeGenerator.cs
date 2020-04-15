using System.Collections.Generic;
using MeshDecimator;
using MeshDecimator.Math;
using static System.Math;

namespace RockGen.Primitive
{
public class SphereCubeGenerator
{
    public int NumSubDivX
    {
        get => numSubDivX;
        set
        {
            stateInvalidated = value != numSubDivX;
            numSubDivX       = value;
        }
    }

    public int NumSubDivY
    {
        get => numSubDivY;
        set
        {
            stateInvalidated = value != numSubDivY;
            numSubDivY       = value;
        }
    }

    public int NumSubDivZ
    {
        get => numSubDivZ;
        set
        {
            stateInvalidated = value != numSubDivZ;
            numSubDivZ       = value;
        }
    }

    public float Radius
    {
        get => radius;
        set
        {
            stateInvalidated = Abs(value - radius) < 1e-4f;
            radius           = value;
        }
    }

    private float radius;

    private int numSubDivX = 8;
    private int numSubDivY = 8;
    private int numSubDivZ = 8;

    int verticesCount;

    bool stateInvalidated = true;

    readonly List<Vector3d> vertices  = new List<Vector3d>();
    readonly List<Vector3>  normals   = new List<Vector3>();
    readonly List<int>      triangles = new List<int>();

    int GetVerticesCount()
    {
        const int cornerVertices = 8;

        int edgeVertices = (NumSubDivX + NumSubDivY + NumSubDivZ - 3) * 4;

        int faceVertices = ((NumSubDivX - 1) * (NumSubDivY - 1) +
                            (NumSubDivX - 1) * (NumSubDivZ - 1) +
                            (NumSubDivY - 1) * (NumSubDivZ - 1)) * 2;

        return cornerVertices + edgeVertices + faceVertices;
    }

    private void UpdateState()
    {
        if (!stateInvalidated) return;

        verticesCount = GetVerticesCount();

        vertices.Clear();
        normals.Clear();
        triangles.Clear();

        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertex(int x, int y, int z)
    {
        var   xCoord = x * 2f / numSubDivX - 1f;
        var   yCoord = y * 2f / numSubDivY - 1f;
        var   zCoord = z * 2f / numSubDivZ - 1f;
        float xSqr   = xCoord * xCoord;
        float ySqr   = yCoord * yCoord;
        float zSqr   = zCoord * zCoord;

        // https://math.stackexchange.com/questions/118760/can-someone-please-explain-the-cube-to-sphere-mapping-formula-to-me
        Vector3d normal = new Vector3d(
            xCoord * Sqrt(1f - ySqr / 2f - zSqr / 2f + ySqr * zSqr / 3f),
            yCoord * Sqrt(1f - xSqr / 2f - zSqr / 2f + xSqr * zSqr / 3f),
            zCoord * Sqrt(1f - xSqr / 2f - ySqr / 2f + xSqr * ySqr / 3f)
        );

        normals.Add(new Vector3(normal));
        vertices.Add(normal * Radius);
    }

    void CreateVertices()
    {
        vertices.Clear();

        // Walls in spiral
        for (int y = 0; y <= NumSubDivY; y++)
        {
            for (int x = 0; x <= NumSubDivX; x++)
                CreateVertex(x, y, 0);

            for (int z = 1; z <= NumSubDivZ; z++)
                CreateVertex(NumSubDivX, y, z);

            for (int x = NumSubDivX - 1; x >= 0; x--)
                CreateVertex(x, y, NumSubDivZ);

            for (int z = NumSubDivZ - 1; z > 0; z--)
                CreateVertex(0, y, z);
        }

        // Ceiling
        for (int z = 1; z < NumSubDivZ; z++)
        for (int x = 1; x < NumSubDivX; x++)
            CreateVertex(x, NumSubDivY, z);

        // Floor
        for (int z = 1; z < NumSubDivZ; z++)
        for (int x = 1; x < NumSubDivX; x++)
            CreateVertex(x, 0, z);
    }

    void AddQuad(int v00, int v10, int v01, int v11)
    {
        triangles.Add(v00);
        triangles.Add(v01);
        triangles.Add(v10);
        triangles.Add(v10);
        triangles.Add(v01);
        triangles.Add(v11);
    }

    private void CreateCeilingFace(int ring)
    {
        int v = ring * NumSubDivY;
        for (int x = 0; x < NumSubDivX - 1; x++, v++)
            AddQuad(v, v + 1, v + ring - 1, v + ring);

        AddQuad(v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (NumSubDivY + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < NumSubDivZ - 1; z++, vMin--, vMid++, vMax++)
        {
            AddQuad(vMin, vMid, vMin - 1, vMid + NumSubDivX - 1);
            for (int x = 1; x < NumSubDivX - 1; x++, vMid++)
                AddQuad(vMid, vMid + 1, vMid + NumSubDivX - 1, vMid + NumSubDivX);

            AddQuad(vMid, vMax, vMid + NumSubDivX - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        AddQuad(vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < NumSubDivX - 1; x++, vTop--, vMid++)
            AddQuad(vMid, vMid + 1, vTop, vTop - 1);

        AddQuad(vMid, vTop - 2, vTop, vTop - 1);
    }

    private void CreateFloorFace(int ringSize)
    {
        int vMid = verticesCount - (NumSubDivX - 1) * (NumSubDivZ - 1);
        AddQuad(ringSize - 1, vMid, 0, 1);
        int v = 1;
        for (int x = 1; x < NumSubDivX - 1; x++, v++, vMid++)
            AddQuad(vMid, vMid + 1, v, v + 1);

        AddQuad(vMid, v + 2, v, v + 1);

        int vMin = ringSize - 2;
        vMid -= numSubDivX - 2;
        int vMax = v + 2;

        for (int z = 1; z < numSubDivZ - 1; z++, vMin--, vMid++, vMax++)
        {
            AddQuad(vMin, vMid + numSubDivX - 1, vMin + 1, vMid);
            for (int x = 1; x < numSubDivX - 1; x++, vMid++)
                AddQuad(vMid + numSubDivX - 1, vMid + numSubDivX, vMid, vMid + 1);

            AddQuad(vMid + numSubDivX - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        AddQuad(vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < numSubDivX - 1; x++, vTop--, vMid++)
            AddQuad(vTop, vTop - 1, vMid, vMid + 1);

        AddQuad(vTop, vTop - 1, vMid, vTop - 2);
    }

    private void CreateTriangles()
    {
        triangles.Clear();

        int ringSize = (NumSubDivX + NumSubDivZ) * 2;
        int v        = 0;

        for (int y = 0; y < NumSubDivY; y++, v++)
        {
            for (int q = 0; q < NumSubDivZ; q++, v++)
                AddQuad(v, v + 1, v + ringSize, v + ringSize + 1);

            for (int q = 0; q < NumSubDivX; q++, v++)
                AddQuad(v, v + 1, v + ringSize, v + ringSize + 1);

            for (int q = 0; q < NumSubDivZ - 1; q++, v++)
                AddQuad(v, v + 1, v + ringSize, v + ringSize + 1);

            for (int q = 0; q < NumSubDivX; q++, v++)
                AddQuad(v, v + 1, v + ringSize, v + ringSize + 1);

            AddQuad(v, v - ringSize + 1, v + ringSize, v + 1);
        }

        CreateCeilingFace(ringSize);
        CreateFloorFace(ringSize);
    }

    public Mesh MakeSphere()
    {
        UpdateState();

        var mesh = new Mesh(vertices.ToArray(), triangles.ToArray()) {
            Normals = normals.ToArray()
        };

        return mesh;
    }
}
}
