using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralPartGenerator<TTopology, TShape>
    where TTopology : BaseMeshTopologySO
    where TShape : BaseModelShape
{
    public Mesh CreateNewMesh(TTopology topology)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        GeneratePols(ref vertices, ref triangles, ref uvs, topology);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }
    protected abstract void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, TTopology topo);
    public abstract void ApplyShape(Mesh mesh, TTopology topology, TShape shape);

    // Можно добавить: GetCacheKey, Validate и т.д.
}