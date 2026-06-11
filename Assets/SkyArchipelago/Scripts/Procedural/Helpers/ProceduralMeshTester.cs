using UnityEngine;

[ExecuteInEditMode]
public class ProceduralMeshTester : MonoBehaviour
{
    [Header("Inputs")]
    public BaseMeshTopologySO topology;
    public Color TestColor;

    [Header("Output")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private void OnValidate()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if(meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
    }

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        var gen = new ProceduralMeshService();
        Mesh mesh = gen.GetMesh(topology);

        if (mesh != null)
        {
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = topology.DefShape.Material;
            Debug.Log($"Меш успешно сгенерирован! Вершины: {mesh.vertexCount}");
        }
    }

    [ContextMenu("Update Shape")]
    public void UpdateShape()
    {
        if (meshFilter)
        {
            var gen = new ProceduralMeshService();
            gen.EditMesh(topology, meshFilter.sharedMesh, topology.DefShape);
            meshRenderer.sharedMaterial = topology.DefShape.Material;
        }
    }

    [ContextMenu("ApplyColor")]
    public void ApplyColor()
    {
        var colors = new Color[meshFilter.sharedMesh.vertexCount];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = TestColor;
        }
        meshFilter.sharedMesh.colors = colors;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            DestroyImmediate(meshFilter.sharedMesh);
            meshFilter.sharedMesh = null;
        }
    }
}