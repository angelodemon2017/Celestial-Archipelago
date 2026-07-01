using System;
using Zenject;

public class PlayerFlowService : IInitializable, IDisposable
{
    private DiContainer _container;
    private PlayerConfig _playerConfig;

    private InputService _inputService;

    private SignalBus _signalBus;

    private PlayerFlowService(
        DiContainer container,
        SignalBus signalBus,
        InputService inputService,
        PlayerConfig playerConfig)
    {
        _container = container;
        _signalBus = signalBus;
        _inputService = inputService;
        _playerConfig = playerConfig;
    }

    public void Initialize()
    {
    }

    public void Dispose()
    {

    }
}