using System;
using Zenject;

public class GameplayStateService : IInitializable, ITickable, IFixedTickable, IDisposable
{   
    private readonly DiContainer _container;
    private readonly SignalBus _signalBus;

    private readonly HinterService _hinterService;
    private readonly InputService _inputService;
    private readonly CursorService _cursorService;

    private BaseGameplayState _currentState;

    public GameplayStateService(
        DiContainer container,
        SignalBus signalBus,
        HinterService hinterService,
        InputService inputService,
        CursorService cursorService)
    {
        _container = container;
        _signalBus = signalBus;
        _hinterService = hinterService;
        _inputService = inputService;
        _cursorService = cursorService;
    }

    public void Initialize()
    {

    }

    public void Tick()
    {
        _currentState?.StateRun();
    }

    public void FixedTick()
    {
        _currentState?.StateFixedRun();
    }

    public void SetState<T>() where T : BaseGameplayState
    {
        SetState(_container.Resolve<T>());
    }

    private void SetState(BaseGameplayState nextState)
    {
        if (_currentState != null)
        {
            _currentState.StateOff();
        }

        _currentState = nextState;
        _hinterService.SetSourceHint(_currentState);
        _inputService.SetInputProviderContainer(_currentState);
        if (_currentState.CursorIsAvailable)
            _cursorService.UnlockCursor();
        else
            _cursorService.LockCursor();
        _currentState.StateOn();
    }

    public void Dispose()
    {

    }
}