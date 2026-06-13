public class ManagerMenuState : StateWithWindow<MenuOfManagerView>
{
    private readonly GameplayStateService _gameplayStateService;

    public ManagerMenuState(
        GameplayStateService gameplayStateService,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        _gameplayStateService = gameplayStateService;
    }

    public override void ProcessTab(bool interact)
    {
        if (interact)
            _gameplayStateService.SetState<MainFPSState>();
    }
}