using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoadingService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly CoroutineRunner _coroutineRunner;

    private Coroutine _currentLoadingCoroutine;
    private string _currentLoadingScene;

    [Inject]
    public SceneLoadingService(SignalBus signalBus, CoroutineRunner coroutineRunner)
    {
        _signalBus = signalBus;
        _coroutineRunner = coroutineRunner;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LoadSceneSignal>(OnLoadSceneRequested);

        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Menu));
    }

    private void OnLoadSceneRequested(LoadSceneSignal signal)
    {
        SceneManager.LoadScene(signal.SceneName, signal.Mode);
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