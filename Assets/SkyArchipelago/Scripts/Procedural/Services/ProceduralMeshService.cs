using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshService
{
    private Dictionary<BaseMeshTopologySO, Queue<Mesh>> _meshPool = new();
    private Dictionary<BaseMeshTopologySO, ProceduralMeshGenerator> _mapGenerator = new();

    private void CheckAndRegisterTopology(BaseMeshTopologySO baseMeshTopology)
    {
        if (!_meshPool.ContainsKey(baseMeshTopology))
        {
            _meshPool.Add(baseMeshTopology, new Queue<Mesh>());
        }
        if (!_mapGenerator.ContainsKey(baseMeshTopology))
        {
            _mapGenerator.Add(baseMeshTopology, baseMeshTopology.MeshGenerator);
        }
    }

    public Mesh GetMesh(BaseMeshTopologySO baseMeshTopology)
    {
        CheckAndRegisterTopology(baseMeshTopology);

        if (_meshPool.TryGetValue(baseMeshTopology, out Queue<Mesh> queueMesh) &&
            queueMesh.TryDequeue(out Mesh result))
        {
            return result;
        }

        return _mapGenerator[baseMeshTopology].CreateNewMesh();
    }

    public void EditMesh(BaseMeshTopologySO baseMeshTopology, Mesh mesh, BaseModelShape baseModelShape)
    {
        CheckAndRegisterTopology(baseMeshTopology);

        _mapGenerator[baseMeshTopology].ApplyShape(mesh, baseModelShape);
    }

    public void ReturnMesh(BaseMeshTopologySO baseMeshTopology, Mesh mesh)
    {
        CheckAndRegisterTopology(baseMeshTopology);

        _meshPool[baseMeshTopology].Enqueue(mesh);
    }
}