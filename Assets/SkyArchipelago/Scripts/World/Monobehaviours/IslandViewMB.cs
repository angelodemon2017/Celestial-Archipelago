using UnityEngine;

public class IslandViewMB : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Transform _rootIsland;

    private IslandData _islandData;

    public void InitAndUpdate(IslandData islandData, Mesh mesh)
    {
        _islandData = islandData;
        UpdateView(mesh, _islandData.Center);
    }

    public void UpdateView(Mesh mesh, Vector3 localPosition)
    {
        _rootIsland.localPosition = localPosition;
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }
}