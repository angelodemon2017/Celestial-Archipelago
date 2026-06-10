using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class IslandGenerator : MonoBehaviour
{
    [Header("Config")]
    public IslandTopologySO topology;
    public IslandShape shape;

    [Header("Components")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public NavMeshSurface navMeshSurface;

    [ContextMenu("Generate Island")]
    public void GenerateIsland()
    {
        if (topology == null || shape == null) return;

        ProceduralMeshService pms = new ProceduralMeshService();

        // 1. Получаем базовый меш из пула
        Mesh mesh = pms.GetMesh(topology);

        // 2. Применяем shape (шум, вариации)
        pms.EditMesh(topology, mesh, shape);

        // 3. Применяем на сцену
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        if (shape.Material != null)
            meshRenderer.sharedMaterial = shape.Material;

        // 4. Bake NavMesh
        BakeNavMesh();
    }

    private void BakeNavMesh()
    {
        if (navMeshSurface == null)
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();

        navMeshSurface.collectObjects = CollectObjects.Children; // или Volume
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders; // рекомендуется для proxy

        // Опционально: добавить простой BoxCollider как proxy для верхнего плато
        AddNavProxyColliders();

        navMeshSurface.BuildNavMesh();
        Debug.Log("Island + NavMesh generated");
    }

    private void AddNavProxyColliders()
    {
        // Добавляем большой box для верхнего плато (быстрый и надёжный NavMesh)
        var plateauBox = gameObject.AddComponent<BoxCollider>();
        plateauBox.center = new Vector3(0, topology.plateauHeight * 0.5f, 0);
        plateauBox.size = new Vector3(topology.plateauRadius * 2f, topology.plateauHeight * 1.2f, topology.plateauRadius * 2f);
    }
}