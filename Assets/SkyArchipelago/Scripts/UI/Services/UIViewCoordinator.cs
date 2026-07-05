using UnityEngine;
using Zenject;

public class UIViewCoordinator
{
    private DiContainer _container;
    private UIWindowBase _mainRootWindow;

    private Canvas _rootCanvas;

    public UIViewCoordinator(
        DiContainer container,
        [Inject(Id = Dicts.DiPrefabIds.Canvas)] Canvas canvasPrefab)
    {
        _container = container;
        _rootCanvas = GameObject.Instantiate(canvasPrefab);
        GameObject.DontDestroyOnLoad(_rootCanvas);
    }

    public TState ChangeWindow<TState>()
        where TState : UIWindowBase
    {
        var nextWindow = _container.Resolve<TState>();
        nextWindow.transform.SetParent(_rootCanvas.transform, false);

        ShowWindowAsync(nextWindow);
        return nextWindow;
    }

    private void ShowWindowAsync(UIWindowBase window)
    {
        if (window == _mainRootWindow) return;
        if (_mainRootWindow != null)
        {
            _mainRootWindow.Hide();
        }

        _mainRootWindow = window;
        _mainRootWindow.Show();
    }
}