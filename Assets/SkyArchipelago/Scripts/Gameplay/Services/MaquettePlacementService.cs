using UnityEngine;
using Zenject;

public class MaquettePlacementService
{
    private readonly SignalBus _signalBus;
    private readonly BuildFPSStateConfig _buildFPSStateConfig;
    private readonly EntitiesCatalogManager _catalogManager;
    private readonly DataService _dataService;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly EntityRecipeCatalogManager _entityRecipeCatalogManager;
    private readonly WorldShowerService _worldShowerService;
    private readonly BuildingModel _buildingModel;
    private readonly GameplayLocalFPSModel _gameplayLocalFPSModel;
    private readonly MaquetteReleaseService _maquetteReleaseService;
    private readonly FPSCommonModel _fPSCommonModel;

    private EntityRecipeConfig _entityRecipeConfig;
    private EntityModel _maquetteOfEntityModel;
    private EntityViewMB _maquetteView;
    private PlacementEntityRootHandlerMB _placementEntityRootHandler;
    private bool _validPlacement;
    private int _angleYSwift;

    public EEntityType CurrentEntity => _entityRecipeConfig?.EntityConfig?.eEntityType ?? EEntityType.None;
    private int _powerRotating => 5;

    public MaquettePlacementService(
        SignalBus signalBus,
        DataService dataService,
        BuildFPSStateConfig buildFPSStateConfig,
        FPSCommonModel fPSCommonModel,
        EntityRecipeCatalogManager entityRecipeCatalogManager,
        EntityRuntimeService entityRuntimeService,
        EntitiesCatalogManager catalogManager,
        WorldShowerService worldShowerService,
        BuildingModel buildingModel,
        GameplayLocalFPSModel gameplayLocalFPSModel,
        MaquetteReleaseService maquetteReleaseService)
    {
        _signalBus = signalBus;
        _buildFPSStateConfig = buildFPSStateConfig;
        _catalogManager = catalogManager;
        _fPSCommonModel = fPSCommonModel;
        _entityRecipeCatalogManager = entityRecipeCatalogManager;
        _entityRuntimeService = entityRuntimeService;
        _dataService = dataService;
        _worldShowerService = worldShowerService;
        _buildingModel = buildingModel;
        _gameplayLocalFPSModel = gameplayLocalFPSModel;
        _maquetteReleaseService = maquetteReleaseService;
    }

    public void StartPlacement()
    {
        if (!_entityRecipeCatalogManager.TryGetConfigByKey(_buildingModel.RecipeId, out var recipe))
            return;

        _entityRecipeConfig = recipe;
        _angleYSwift = 0;
        var maquetteData = (MaquetteOfEntityData)EntityDataMap.CreateData(EEntityType.MaquetteEntity);
        maquetteData.InitConfig(_buildFPSStateConfig.ConfigBaseMaquette);
        maquetteData.EntityId = _buildingModel.IdEntity;
        maquetteData.RecipeId = _buildingModel.RecipeId;
        _dataService.worldData.StaticIslands.Datas[0].entities.AddNewData(maquetteData);
        _maquetteOfEntityModel = _entityRuntimeService.CreateEntityModel(maquetteData);
        _entityRuntimeService.AddModel(_maquetteOfEntityModel);
        _maquetteView = _worldShowerService.SpawnViewModelEntity(_maquetteOfEntityModel);
        if (_maquetteView.EntityRootHandler is PlacementEntityRootHandlerMB perh)
        {
            _placementEntityRootHandler = perh;
            _placementEntityRootHandler.ShowValidState(_placementEntityRootHandler.CountConflictEntities == 0);
        }
    }

    public void UpdatePositionMaquette(Vector3 newPos)
    {
        _maquetteView.transform.position = newPos;
        var eulRot = _gameplayLocalFPSModel.LocalPlayerView.transform.rotation.eulerAngles;
        eulRot.y += _angleYSwift;
        _maquetteView.transform.rotation = Quaternion.Euler(eulRot);
        CheckAndUpdateValid();
    }

    public void UpdateMaquetteByAnchor(Vector3 newPos, Quaternion rot)
    {
        _maquetteView.transform.position = newPos;
        _maquetteView.transform.rotation = rot;
        CheckAndUpdateValid();
    }

    private void CheckAndUpdateValid()
    {
        var validPlace = _placementEntityRootHandler.CountConflictEntities == 0;
        if (validPlace != _validPlacement)
        {//maybe in modelEntity
            _validPlacement = validPlace;
            _placementEntityRootHandler.ShowValidState(_validPlacement);
        }
    }

    public void UpdateYSwift(float swift)
    {
        _angleYSwift += swift > 0 ? _powerRotating : -_powerRotating;
    }

    public bool TrySetPosition()
    {
        if (!_validPlacement)
            return false;

        _maquetteOfEntityModel.Position = _maquetteView.transform.position;
        _maquetteOfEntityModel.Rotation = _maquetteView.transform.rotation;

        if (!_entityRecipeConfig.IsDeleteInputsOnPlacement ||
            _maquetteReleaseService.TryReleaseRecipeEntity(_maquetteOfEntityModel.Id, _fPSCommonModel.ContainerModel.Id))
        {
            SetNullRefs();
            return true;
        }
        return false;
    }

    public void CancelPlacement()
    {
        if (_maquetteOfEntityModel != null)
        {
            _signalBus.Fire(new EntityDeleteRequestSignal(_maquetteOfEntityModel.Id, -1));
            SetNullRefs();
        }
    }

    private void SetNullRefs()
    {
        _maquetteOfEntityModel = null;
        _maquetteView = null;
        _placementEntityRootHandler = null;
    }
}