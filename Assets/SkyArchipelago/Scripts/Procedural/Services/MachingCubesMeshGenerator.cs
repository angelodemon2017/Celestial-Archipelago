using MarchingCubesProject;
using System.Collections.Generic;
using UnityEngine;

public class MachingCubesMeshGenerator
{
    public Mesh GenerateMeshFromDensity(float[,,] density, float surfaceLevel, float cellSize, MarchingCubesConfigSO config)
    {
        int width = density.GetLength(0);
        int height = density.GetLength(1);
        int depth = density.GetLength(2);

        var voxelArray = new VoxelArray(width, height, depth);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                    voxelArray[x, y, z] = density[x, y, z];

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        var marching = new MarchingCubes();
        marching.Surface = surfaceLevel;
        marching.Generate(voxelArray.Voxels, verts, indices);

        List<Color> colors = new List<Color>(verts.Count);
        for (int i = 0; i < verts.Count; i++)
        {
            Color col = config.GetVertexColor(verts[i]);
            colors.Add(col);
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
}