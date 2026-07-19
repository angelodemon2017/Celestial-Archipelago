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
        GameplayLocalFPSModel gameplayLocalFPSModel,
        UIViewCoordinator uIViewCoordinator,
        GameplayStateService gameplayStateService,
        RaycastService raycastService,
        CameraService cameraService,
        EntityRuntimeService entityRuntimeService,
        PlayerInteractionService playerInteractionService,
        EntityViewsFactory entityViewsFactory) :
        base (
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
        KeyHints.Add("E - Interact");
        KeyHints.Add("LMB - MainAction");
        KeyHints.Add("RMB - Harvesting");
        KeyHints.Add("Tab - Manager menu");
        _gameplayStateService = gameplayStateService;
    }

    public override void ProcessLeftMouseButton(bool lmb)
    {
        if (lmb)
            _playerInteractionService.TryMainAction();
    }

    public override void ProcessRightMouseButton(bool rmb)
    {
        if(rmb)
            _playerInteractionService.TryAltAction();
    }

    public override void ProcessInteract(bool interacted)
    {
        if (interacted)
            _playerInteractionService.TryInteractWithHandler();
    }

    public override void ProcessTab(bool interact)
    {
        if(interact)
            _gameplayStateService.SetState<ManagerMenuState>();
    }
}