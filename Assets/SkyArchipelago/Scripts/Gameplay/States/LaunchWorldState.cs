using Zenject;

public class LaunchWorldState : BaseGameplayState
{
    private readonly SignalBus _signalBus;
    private readonly DataService _dataService;
    private readonly WorldShowerService _worldShowerService;

    public LaunchWorldState(
        SignalBus signalBus,
        DataService dataService,
        WorldShowerService worldShowerService)
    {
        _signalBus = signalBus;
        _dataService = dataService;
        _worldShowerService = worldShowerService;
    }

    public override void StateOn()
    {
        base.StateOn();
        _signalBus.Subscribe<SceneInstalledSignal>(OnHandle);
    }

    public override void StateOff()
    {
        _signalBus.Unsubscribe<SceneInstalledSignal>(OnHandle);
    }

    private void OnHandle(SceneInstalledSignal sceneInstalled)
    {
        _worldShowerService.ShowChunk(0, 0);//get current chunk from data
    }
}