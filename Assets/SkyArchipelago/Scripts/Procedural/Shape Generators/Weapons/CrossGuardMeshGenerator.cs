using System.Collections.Generic;
using UnityEngine;

public class CrossGuardMeshGenerator : ProceduralMeshGeneratorByConfig<GuardTopologySO>
{
    public CrossGuardMeshGenerator(GuardTopologySO topConfig) : base(topConfig) { }

    public override void ApplyShape<T>(Mesh mesh, T model)
    {
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            //apply shape
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public override Mesh CreateNewMesh()
    {
        // Основная горизонтальная перекладина
        Mesh mainBar = CreateBox(_topology.guardModelShape.guardWidth, _topology.guardModelShape.guardThickness * 1.6f, _topology.guardModelShape.guardThickness * 3.2f);

        // Вертикальная часть
        Mesh vertical = CreateBox(_topology.guardModelShape.guardThickness * 2.4f, _topology.guardModelShape.guardHeight, _topology.guardModelShape.guardThickness * 1.4f);

        // Дополнительные "крылья"/детали на концах (как на твоём рисунке)
        Mesh leftWing = CreateBox(_topology.guardModelShape.guardThickness * 1.8f, _topology.guardModelShape.guardThickness * 2.2f, _topology.guardModelShape.guardThickness * 1.2f);
        Mesh rightWing = CreateBox(_topology.guardModelShape.guardThickness * 1.8f, _topology.guardModelShape.guardThickness * 2.2f, _topology.guardModelShape.guardThickness * 1.2f);

        CombineInstance[] combine = new CombineInstance[5];
        combine[0].mesh = mainBar;
        combine[0].transform = Matrix4x4.identity;

        combine[1].mesh = vertical;
        combine[1].transform = Matrix4x4.Translate(new Vector3(0, _topology.guardModelShape.guardThickness * 0.3f, 0));

        // Левое крыло
        combine[2].mesh = leftWing;
        combine[2].transform = Matrix4x4.Translate(new Vector3(-_topology.guardModelShape.guardWidth * 0.45f, _topology.guardModelShape.guardThickness * 0.4f, 0));

        // Правое крыло
        combine[3].mesh = rightWing;
        combine[3].transform = Matrix4x4.Translate(new Vector3(_topology.guardModelShape.guardWidth * 0.45f, _topology.guardModelShape.guardThickness * 0.4f, 0));

        // Центральный элемент
        combine[4].mesh = CreateBox(_topology.guardModelShape.guardThickness * 2.8f, _topology.guardModelShape.guardThickness * 2.8f, _topology.guardModelShape.guardThickness * 2.8f);
        combine[4].transform = Matrix4x4.Translate(new Vector3(0, _topology.guardModelShape.guardThickness * 0.2f, 0));

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine, true, true);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateBox(float width, float height, float depth)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float hw = width * 0.5f;
        float hh = height * 0.5f;
        float hd = depth * 0.5f;

        // 8 вершин куба
        vertices.Add(new Vector3(-hw, -hh, hd)); // 0
        vertices.Add(new Vector3(hw, -hh, hd)); // 1
        vertices.Add(new Vector3(hw, hh, hd)); // 2
        vertices.Add(new Vector3(-hw, hh, hd)); // 3

        vertices.Add(new Vector3(-hw, -hh, -hd)); // 4
        vertices.Add(new Vector3(hw, -hh, -hd)); // 5
        vertices.Add(new Vector3(hw, hh, -hd)); // 6
        vertices.Add(new Vector3(-hw, hh, -hd)); // 7

        // Простые UV (можно улучшить позже)
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < 8; i++) uvs.Add(new Vector2(0f, 0f));

        int[] tris = {
            0,1,2,  0,2,3,   // front
            5,4,7,  5,7,6,   // back
            4,0,3,  4,3,7,   // left
            1,5,6,  1,6,2,   // right
            3,2,6,  3,6,7,   // top
            4,5,1,  4,1,0    // bottom
        };

        triangles.AddRange(tris);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}