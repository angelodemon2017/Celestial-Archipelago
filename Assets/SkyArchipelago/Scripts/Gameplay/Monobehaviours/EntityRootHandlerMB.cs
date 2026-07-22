using UnityEngine;
using Zenject;

public class EntityRootHandlerMB : MonoBehaviour, IPoolable<EntityModel>
{
    [SerializeField] private HitDetector _hitDetector;
    [SerializeField] protected InteractHandlerMB _interactHandler;

    private HitDetectorsMap _hitsCoordinatorService;
    protected InteractHandlersRegistry _interactableCoordinatorService;
    
    public EntityModel _entityModel;//private

    public EntityModel EntityModel => _entityModel;
    public InteractHandlerMB InteractHandler => _interactHandler;

    protected virtual void Awake()
    {
        
    }

    [Inject]
    public void Construct(
        HitDetectorsMap hitsCoordinatorService,
        InteractHandlersRegistry interactableCoordinatorService)
    {
        _hitsCoordinatorService = hitsCoordinatorService;
        _interactableCoordinatorService = interactableCoordinatorService;
    }

    public void OnSpawned(EntityModel p1)
    {
        Init(p1);
        RegisterEntity();
    }

    public virtual void Init(EntityModel model)
    {
        _entityModel = model;
        _interactHandler?.SetModel(model);
    }

    private void RegisterEntity()
    {
        if (_interactHandler && _interactableCoordinatorService != null)
            for (int i = 0; i < _interactHandler.GOsOfColliders.Count; i++)
                _interactableCoordinatorService.Register(_interactHandler.GOsOfColliders[i], _interactHandler);
        if (_hitDetector && _entityModel != null && _hitsCoordinatorService != null)
            _hitsCoordinatorService.Register(_hitDetector.gameObject, _entityModel);
    }

    public virtual void OnDespawned()
    {
        Inregister();
        _entityModel = null;
        _interactHandler?.SetModel(null);
    }

    private void Inregister()
    {
        if (_interactHandler)
            _interactableCoordinatorService.Unregister(_interactHandler.gameObject);
        if (_hitDetector)
            _hitsCoordinatorService.Unregister(_hitDetector.gameObject);
    }

    public virtual void OnDestroy()
    {
    }
}