using UnityEngine;
using Zenject;

public class EntityViewMB : MonoBehaviour, IPoolable<EntityRootHandlerMB>
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _rootEntity;

    private EntityRootHandlerMB _entityRootHandler;
    private EntityModel _entityModel;

    public int GetConfigId => _entityModel.ConfigId;
    public EntityRootHandlerMB EntityRootHandler => _entityRootHandler;
    public Rigidbody RB => _rb;

    public void OnSpawned(EntityRootHandlerMB p1)
    {
        _entityRootHandler = p1;
        _entityModel = p1.EntityModel;
        Init(_entityModel);
        UpdateView();
    }

    public void Init(EntityModel entityModel)
    {
        _entityModel = entityModel;
        _rb.isKinematic = !_entityModel.IsPhysical;
        _rb.useGravity = _entityModel.IsPhysical;
    }

    private void UpdateView()
    {
        _entityRootHandler.transform.SetParent(_rootEntity);
        _entityRootHandler.transform.localPosition = Vector3.zero;
        _entityRootHandler.transform.localRotation = Quaternion.identity;
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

    public void OnDespawned()
    {
        _entityRootHandler.OnDespawned();
        _entityRootHandler = null;
        _entityModel = null;
    }
}