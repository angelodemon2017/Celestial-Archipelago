public class MenuOfEntityWithInventoryState : StateWithWindow<MenuOfEntityWithInventoryView>
{
    private readonly GameplayStateService _gameplayStateService;
    private readonly MenuEntityWithInventoryModel _menuEntityWithInventoryModel;

    public MenuOfEntityWithInventoryState(
        GameplayStateService gameplayStateService,
        MenuEntityWithInventoryModel menuEntityWithInventoryModel,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        _gameplayStateService = gameplayStateService;
        _menuEntityWithInventoryModel = menuEntityWithInventoryModel;
    }

    public override void StateOn()
    {
        base.StateOn();
        _menuEntityWithInventoryModel.OnTryClosed += GoToMainGameplay;
    }

    public override void ProcessTryClose(bool isClosing)
    {
        if (isClosing)
            GoToMainGameplay();
    }

    private void GoToMainGameplay()
    {
        _gameplayStateService.SetState<MainFPSState>();
    }

    public override void StateOff()
    {
        base.StateOff();
        _menuEntityWithInventoryModel.OnTryClosed -= GoToMainGameplay;
        _menuEntityWithInventoryModel.CleanByExitState();
    }
}