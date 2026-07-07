using UnityEngine;
using Zenject;

public class BuildingFPSState : BaseFPSState<GameplayControllerView>
{
    private readonly GameplayStateService _gameplayStateService;
    private readonly BuildingModel _buildingModel;

    private BuildMarkerMB _buildMarker;

    public override bool CursorIsAvailable => false;

    protected override LayerMask RaycastLayer => _playerConfig.FPSBuildingLayer;

    public BuildingFPSState(
        DiContainer container,
        SignalBus signalBus,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
        BuildingModel buildingModel,
        UIViewCoordinator uIViewCoordinator,
        GameplayStateService gameplayStateService,
        RaycastService raycastService,
        CameraService cameraService,
        EntityRuntimeService entityRuntimeService,
        PlayerInteractionService playerInteractionService,
        EntityViewsFactory entityViewsFactory) :
        base(
            container,
            playerConfig,
            fPSCommonModel,
            raycastService,
            cameraService,
            entityRuntimeService,
            playerInteractionService,
            entityViewsFactory,
            uIViewCoordinator)
    {
        _buildingModel = buildingModel;
        _gameplayStateService = gameplayStateService;
    }

    public override void StateOn()
    {
        base.StateOn();

        CheckBuildMarker();
    }

    public override void StateRun()
    {
        base.StateRun();

        UpdateMarker();
    }

    private void UpdateMarker()
    {
        if (!_buildMarker)
            return;

        _buildMarker.transform.position = _playerInteractionService.LastHit;
    }

    private void CheckBuildMarker()
    {
        if (_buildMarker)
        {
            _buildMarker.gameObject.SetActive(true);
        }
        else
        {
            _buildMarker = _container.Resolve<BuildMarkerMB>();
        }
    }

    public override void ProcessTryClose(bool isClosing)
    {
        if (isClosing)
            _gameplayStateService.SetState<MainFPSState>();
    }

    public override void StateOff()
    {
        base.StateOff();

//        if(_buildMarker)
//            _buildMarker.gameObject.SetActive(false);
    }
}