using Zenject;

public class MainMenuState : StateWithWindow<MainMenuControllerView>
{
    private readonly SignalBus _signalBus;
    private readonly MainMenuModel _mainMenuModel;
    private readonly WorldGeneratorService _worldGeneratorService;

    public MainMenuState(
        SignalBus signalBus,
        MainMenuModel mainMenuModel,
        WorldGeneratorService worldGeneratorService,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        _signalBus = signalBus;
        _mainMenuModel = mainMenuModel;
        _worldGeneratorService = worldGeneratorService;
    }

    public override void StateOn()
    {
        base.StateOn();
        _mainMenuModel.OnNewGameClick += NewGame;
    }

    public override void StateOff()
    {
        _mainMenuModel.OnNewGameClick -= NewGame;
    }

    private void NewGame()
    {
        _worldGeneratorService.GenerateNewWorld();
        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Island));
    }
}