using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMeshTopologySO : ScriptableObject
{
    public virtual BaseModelShape DefShape { get; }

    public abstract ProceduralMeshGenerator MeshGenerator { get; }

    public virtual void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {

    }
}