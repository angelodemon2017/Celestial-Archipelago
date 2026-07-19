using UnityEngine;
using Zenject;

public class BuildingFPSState : BaseFPSState<GameplayControllerView>
{
    private readonly GameplayStateService _gameplayStateService;
    private readonly MaquettePlacementService _maquettePlacementService;
    private readonly BuildingModel _buildingModel;

    private BuildMarkerMB _buildMarker;

    public override bool CursorIsAvailable => false;

    protected override LayerMask RaycastLayer => _playerConfig.FPSBuildingLayer;

    public BuildingFPSState(
        DiContainer container,
        SignalBus signalBus,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
        GameplayLocalFPSModel gameplayLocalFPSModel,
        BuildingModel buildingModel,
        UIViewCoordinator uIViewCoordinator,
        GameplayStateService gameplayStateService,
        RaycastService raycastService,
        CameraService cameraService,
        EntityRuntimeService entityRuntimeService,
        PlayerInteractionService playerInteractionService,
        MaquettePlacementService maquettePlacementService,
        EntityViewsFactory entityViewsFactory) :
        base(
            container,
            playerConfig,
            fPSCommonModel,
            gameplayLocalFPSModel,
            raycastService,
            cameraService,
            entityRuntimeService,
            playerInteractionService,
            entityViewsFactory,
            uIViewCoordinator)
    {
        _buildingModel = buildingModel;
        _gameplayStateService = gameplayStateService;
        _maquettePlacementService = maquettePlacementService;
    }

    public override void StateOn()
    {
        base.StateOn();
        _maquettePlacementService.StartPlacement();
        CheckBuildMarker();
    }

    public override void StateRun()
    {
        base.StateRun();
        _maquettePlacementService.UpdatePositionMaquette(_playerInteractionService.LastHit);
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

    public override void ProcessTab(bool interact)
    {
        if(interact)
            _gameplayStateService.SetState<MainFPSState>();
    }

    public override void ProcessScrollMouse(float scroll)
    {
        if (scroll != 0)
            _maquettePlacementService.UpdateYSwift(scroll);
    }

    public override void ProcessLeftMouseButton(bool lmb)
    {
        if (lmb)
        {
            if(_maquettePlacementService.TrySetPosition())
                _gameplayStateService.SetState<MainFPSState>();
        }
    }

    public override void StateOff()
    {
        base.StateOff();
        _maquettePlacementService.CancelPlacement();
    }
}