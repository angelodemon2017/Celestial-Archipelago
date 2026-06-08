using UnityEngine;

[ExecuteInEditMode]
public class ProceduralMeshTester : MonoBehaviour
{
    [Header("Inputs")]
    public BaseMeshTopologySO topology;

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
        var gen = new HandleOfWeaponGenerator();
        Mesh mesh = gen.CreateNewMesh((HandleTopologySO)topology);

        if (mesh != null)
        {
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = topology.DefShape.material;
            Debug.Log($"Меш успешно сгенерирован! Вершины: {mesh.vertexCount}");
        }
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