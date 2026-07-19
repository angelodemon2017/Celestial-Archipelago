using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlacementEntityRootHandlerMB : EntityRootHandlerMB
{
    [SerializeField] private Transform _root;

    [Inject] private BuildFPSStateConfig _buildFPSStateConfig;
    [Inject] private EntitiesCatalogManager _entitiesCatalogManager;
    [Inject] private EntityViewsFactory _entityViewsFactory;

    private int _uid;
    private MaquetteOfEntityModel _model;
    private BasePlacebleEntityRootHandlerMB _entityRoot;
    private HashSet<GameObject> _conflictEntities = new();

    public int CountConflictEntities => _conflictEntities.Count;

    public override void Init(EntityModel model)
    {
        base.Init(model);
        if (model is MaquetteOfEntityModel maquette)
            ShowGhostModel(maquette);
    }

    private void TriggerEnterEntity(GameObject gameObject)
    {
        _conflictEntities.Add(gameObject);
    }

    private void TriggerExitEntity(GameObject gameObject)
    {
        _conflictEntities.Remove(gameObject);
    }

    private void ShowGhostModel(MaquetteOfEntityModel maquette)
    {
        _model = maquette;
        if (!_entitiesCatalogManager.TryGetConfigByKey(_model.GetIdEntity, out var config))
            return;

        _uid = config.modelConfig.Uid;
        var erh = _entityViewsFactory.SpawnEntityRootHandler(config);
        if (!(erh is BasePlacebleEntityRootHandlerMB entityRoot))
            return;

        _conflictEntities.Clear();
        _entityRoot = entityRoot;
        _entityRoot.transform.SetParent(_root);
        _entityRoot.transform.localPosition = Vector3.zero;
        _entityRoot.transform.localRotation = Quaternion.identity;
        _entityRoot.EnableGhostView(_buildFPSStateConfig.ValidMaterial);
        _entityRoot.TriggerDetector.TriggerEntered += TriggerEnterEntity;
        _entityRoot.TriggerDetector.TriggerExited += TriggerExitEntity;
        for (int i = 0; i < _entityRoot.InteractHandler.GOsOfColliders.Count; i++)
            _interactableCoordinatorService.Register(_entityRoot.InteractHandler.GOsOfColliders[0], _interactHandler);
    }

    public void ShowValidState(bool on)
    {
        _entityRoot.EnableGhostView(
            on ? _buildFPSStateConfig.ValidMaterial :
            _buildFPSStateConfig.InValidMaterial);
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        for (int i = 0; i < _entityRoot.InteractHandler.GOsOfColliders.Count; i++)
            _interactableCoordinatorService.Unregister(_entityRoot.InteractHandler.GOsOfColliders[0]);
        _entityRoot.TriggerDetector.TriggerEntered -= TriggerEnterEntity;
        _entityRoot.TriggerDetector.TriggerExited -= TriggerExitEntity;
        _entityRoot.DisableGhost();
        _entityViewsFactory.Despawn(_uid, _entityRoot);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}