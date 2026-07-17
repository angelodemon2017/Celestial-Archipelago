public class ManagerMenuState : StateWithWindow<MenuOfManagerView>
{
    private readonly GameplayStateService _gameplayStateService;
    private readonly BuildingModel _buildingModel;

    public ManagerMenuState(
        BuildingModel buildingModel,
        GameplayStateService gameplayStateService,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        KeyHints.Add("Q - Close this menu");
        KeyHints.Add("E - Close this menu");
        _buildingModel = buildingModel;
        _gameplayStateService = gameplayStateService;
    }

    public override void ProcessTab(bool interact)
    {
        if (interact)
            _gameplayStateService.SetState<MainFPSState>();
    }

    public override void ProcessTryClose(bool isClosing)
    {
        if (isClosing)
            _gameplayStateService.SetState<MainFPSState>();
    }

    public override void StateOn()
    {
        base.StateOn();
        _buildingModel.SelectedStruct += StartBuild;
    }

    private void StartBuild()
    {
        _gameplayStateService.SetState<BuildingFPSState>();
    }

    public override void StateOff()
    {
        base.StateOff();
        _buildingModel.SelectedStruct -= StartBuild;
    }
}