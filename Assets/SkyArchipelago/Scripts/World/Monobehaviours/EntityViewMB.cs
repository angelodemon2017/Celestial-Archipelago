using UnityEngine;

public class EntityViewMB : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _rootEntity;

    private EntityModel _entityModel;

    public Rigidbody RB => _rb;

    public void InitAndUpdate(EntityModel entityModel, GameObject viewModel)
    {
        _entityModel = entityModel;
        _rb.isKinematic = !_entityModel.IsPhysical;
        _rb.useGravity = _entityModel.IsPhysical;
        UpdateView(viewModel);
    }

    public void UpdateView(GameObject viewModel)
    {
        viewModel.transform.SetParent(_rootEntity);
        viewModel.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (_entityModel == null ||
            !_entityModel.IsPhysical)
            return;

        _entityModel.IsGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}