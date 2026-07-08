using UnityEngine;
using Zenject;

public class EntityRootHandlerMB : MonoBehaviour, IPoolable<EntityModel>
{
    [SerializeField] private HitDetector _hitDetector;
    [SerializeField] private InteractHandlerMB _interactHandler;

    private HitsCoordinatorService _hitsCoordinatorService;
    private EntityModel _entityModel;

    public EntityModel EntityModel => _entityModel;

    [Inject]
    public void Construct(
        HitsCoordinatorService hitsCoordinatorService)
    {
        _hitsCoordinatorService = hitsCoordinatorService;
        RegisterEntity();
    }

    private void RegisterEntity()
    {
        if (_hitsCoordinatorService == null ||
            _entityModel == null)
            return;

        if(_hitDetector)
            _hitsCoordinatorService.Register(_hitDetector.gameObject, _entityModel);
    }

    public void OnSpawned(EntityModel p1)
    {
        Init(p1);
    }

    public void Init(EntityModel model)
    {
        _entityModel = model;
        _interactHandler?.SetModel(model);
    }

    public void OnDespawned()
    {
        _entityModel = null;
        _interactHandler?.SetModel(null);
    }

    public void OnDestroy()
    {
        if (_hitDetector)
            _hitsCoordinatorService.Unregister(_hitDetector.gameObject);
    }
}