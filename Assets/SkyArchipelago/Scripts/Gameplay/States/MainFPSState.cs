using UnityEngine;
using Zenject;

public class MainFPSState : BaseFPSState<GameplayControllerView>
{
    private readonly GameplayStateService _gameplayStateService;

    public override bool CursorIsAvailable => false;

    protected override LayerMask RaycastLayer => _playerConfig.FPSLayer;

    public MainFPSState(
        DiContainer container,
        SignalBus signalBus,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
        UIViewCoordinator uIViewCoordinator,
        PointsRepository pointsRepository,
        GameplayStateService gameplayStateService,
        RaycastService raycastService,
        CameraService cameraService,
        PlayerInteractionService playerInteractionService) :
        base (
            container,
            playerConfig,
            fPSCommonModel,
            pointsRepository,
            raycastService,
            cameraService,
            playerInteractionService,
            uIViewCoordinator)
    {
        _gameplayStateService = gameplayStateService;
    }

    public override void ProcessInteract(bool interacted)
    {
        if (interacted)
            _playerInteractionService.TryInteract();
    }

    public override void ProcessTab(bool interact)
    {
        if(interact)
            _gameplayStateService.SetState<ManagerMenuState>();
    }
}