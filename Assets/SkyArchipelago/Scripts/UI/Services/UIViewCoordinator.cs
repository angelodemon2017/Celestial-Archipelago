using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class UIViewCoordinator : IInitializable, IDisposable, ITickable
{
    private readonly DiContainer _container;
    private readonly SignalBus _signalBus;
    private readonly UIMBFactory<List<string>, ListOfKeyHintsMB> _listKeysFactory;
    private readonly FPSCommonModel _fpsCommonModel;

    private Canvas _rootCanvas;
    //TRASHs for other services
    private Transform _keyHintsParent;
    private List<string> _currentKeyHint;
    private UIWindowBase _mainRootWindow;
    private bool _showKeyHints = false;
    private ListOfKeyHintsMB _locTemp;

    private bool _isDragging;
    private RectTransform _rectTransformOfDrag;
    private TextMeshProUGUI _stringOfDrag;

    public UIViewCoordinator(
        DiContainer container,
        SignalBus signalBus,
        [Inject(Id = Dicts.DiPrefabIds.Canvas)] Canvas canvasPrefab,
        UIMBFactory<List<string>, ListOfKeyHintsMB> listKeys,
        FPSCommonModel fpsCommonModel)
    {
        _container = container;
        _signalBus = signalBus;
        _rootCanvas = GameObject.Instantiate(canvasPrefab);
        _listKeysFactory = listKeys;
        _fpsCommonModel = fpsCommonModel;
        GameObject.DontDestroyOnLoad(_rootCanvas);
        //CRUNCH
        _keyHintsParent = _rootCanvas.transform.GetChild(0);
        _rectTransformOfDrag = (RectTransform)_rootCanvas.transform.GetChild(1);
        _stringOfDrag = _rectTransformOfDrag.GetComponentInChildren<TextMeshProUGUI>();
    }

    public TState ChangeWindow<TState>(List<string> hintKeys)
        where TState : UIWindowBase
    {
        _currentKeyHint = hintKeys;
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
        _mainRootWindow.transform.SetSiblingIndex(0);
        UpdateHints();
    }

    public void ToggleHints()
    {
        _showKeyHints = !_showKeyHints;
        UpdateHints();
    }

    private void UpdateHints()
    {
        if (_locTemp)
        {
            _listKeysFactory.Despawn(_locTemp);
            _locTemp = null;
        }

        if (_showKeyHints)
        {
            _locTemp = _listKeysFactory.Create(_currentKeyHint, _keyHintsParent);
        }
    }

    public void Initialize()
    {
        _signalBus.Subscribe<ContainerOfEntityRequest>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<ContainerOfEntityRequest>(OnHandle);
    }

    private void OnHandle(ContainerOfEntityRequest request)
    {
        if (request.IdContainer == _fpsCommonModel.ContainerDragUIOfPlayer.Id)
        {
            UpdateDrag();
        }
    }

    private void UpdateDrag()
    {
//        Debug.LogError($"Try UpdateDrag");
        var item = _fpsCommonModel.ContainerDragUIOfPlayer.GetItemBySlot(0);
        _isDragging = item.TypeItem != EItemType.None;
        _rectTransformOfDrag.gameObject.SetActive(_isDragging);
        _stringOfDrag.text = _isDragging ? $"{item.FullItemName}({item.Count})" :
            string.Empty;
    }

    public void Tick()
    {
        if (_isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                Input.mousePosition,
                _rootCanvas.worldCamera,
                out Vector2 localPoint
            );

            _rectTransformOfDrag.anchoredPosition = localPoint;
        }
    }
}