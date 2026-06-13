using System;
using UnityEngine;
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

//        Subs();
    }

    public void Initialize()
    {
    }

/*    private void Subs()
    {
        _signalBus.Subscribe<LaunchSpawnPointSignal>(OnHandle);
    }

    private void OnHandle(LaunchSpawnPointSignal launchSpawnPoint)
    {
        if (_playerConfig?.PlayerControllerPrefab != null)
        {
            GameObject playerInstance = _container.InstantiatePrefab(
                _playerConfig.PlayerControllerPrefab,
                launchSpawnPoint.PointPos,
                Quaternion.identity,
                null);

            _container.InjectGameObject(playerInstance);

            var playerController = playerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                _inputService.SetInputProvider(playerController);
            }
        }
        else
        {
            Debug.LogError("PlayerConfig or PlayerPrefab is not set!");
        }
    }/**/

    public void Dispose()
    {
//        _signalBus.Unsubscribe<LaunchSpawnPointSignal>(OnHandle);
    }
}