using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoadingService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly CoroutineRunner _coroutineRunner;
    private readonly GameplayStateService _gameplayStateService;

    private Coroutine _currentLoadingCoroutine;
    private string _currentLoadingScene;

    [Inject]
    public SceneLoadingService(
        SignalBus signalBus,
        CoroutineRunner coroutineRunner,
        GameplayStateService gameplayStateService)
    {
        _signalBus = signalBus;
        _coroutineRunner = coroutineRunner;
        _gameplayStateService = gameplayStateService;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LoadSceneSignal>(OnLoadSceneRequested);

        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Menu));
    }

    private void OnLoadSceneRequested(LoadSceneSignal signal)
    {
        SceneManager.LoadScene(signal.SceneName, signal.Mode);
        switch (signal.SceneName)
        {
            case Dicts.Scenes.Menu:
                _gameplayStateService.SetState<MainMenuState>();
                break;
            case Dicts.Scenes.Island:
                _gameplayStateService.SetState<LaunchWorldState>();
                break;
        }
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LoadSceneSignal>(OnLoadSceneRequested);

        if (_currentLoadingCoroutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_currentLoadingCoroutine);
        }
    }
}