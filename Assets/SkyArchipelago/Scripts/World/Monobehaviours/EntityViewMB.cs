using UnityEngine;

public class EntityViewMB : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Transform _rootEntity;

    private EntityModel _entityModel;

    public void InitAndUpdate(EntityModel entityModel, Mesh mesh)
    {
        _entityModel = entityModel;
        UpdateView(mesh);
    }

    public void InitAndUpdate(EntityModel entityModel, GameObject viewModel)
    {
        _entityModel = entityModel;
        UpdateView(viewModel);
    }

    public void UpdateView(Mesh mesh)
    {
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

    public void UpdateView(GameObject viewModel)
    {
        viewModel.transform.SetParent(_rootEntity);
        viewModel.transform.localPosition = Vector3.zero;
    }
}