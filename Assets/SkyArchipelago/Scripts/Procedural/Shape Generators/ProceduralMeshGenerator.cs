using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshGeneratorByConfig<T> : ProceduralMeshGenerator
    where T : BaseMeshTopologySO
{
    protected T _topology;

    protected ProceduralMeshGeneratorByConfig(T topConfig)
    {
        _topology = topConfig;
    }
}

public class ProceduralMeshGenerator
{
    public virtual Mesh CreateNewMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        GeneratePols(ref vertices, ref triangles, ref uvs);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    protected virtual void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs) { }

    public virtual void ApplyShape<T>(Mesh mesh, T model)
        where T : BaseModelShape
    {

    }
}