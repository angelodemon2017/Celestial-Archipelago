using Zenject;

public class MainMenuState : StateWithWindow<MainMenuControllerView>
{
    private readonly SignalBus _signalBus;
    private readonly MainMenuModel _mainMenuModel;
    private readonly WorldGeneratorService _worldGeneratorService;
    private readonly EntityRuntimeService _entityRuntimeService;

    public MainMenuState(
        SignalBus signalBus,
        MainMenuModel mainMenuModel,
        WorldGeneratorService worldGeneratorService,
        EntityRuntimeService entityRuntimeService,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        _signalBus = signalBus;
        _mainMenuModel = mainMenuModel;
        _worldGeneratorService = worldGeneratorService;
        _entityRuntimeService = entityRuntimeService;
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
        _entityRuntimeService.RunWorld();
        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Island));
    }

    private void LoadGame()
    {
        //Load data of world...
        _entityRuntimeService.RunWorld();
        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Island));
    }
}