using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class InteractionHandlerService : IInitializable, IDisposable
{
    private readonly DiContainer _container;
    private readonly SignalBus _signalBus;

    private Dictionary<EModeInteract, List<BaseInteractHandler>> _handlerByModes = new();
    private int _cacheCount;
    private List<BaseInteractHandler> _cachAllHandlers = new();
    private List<BaseInteractHandler> _validOnCurrentFocus = new();

    public Action ExecutedTarget;

    [Inject]
    public InteractionHandlerService(
        DiContainer container,
        SignalBus signalBus)
    {
        _container = container;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<InteractContext>(OnHandle);

        InitHandlers();
    }

    private void InitHandlers()
    {
        RegisterHandler(_container.Resolve<FullingMaquetteHandler>());
        RegisterHandler(_container.Resolve<DisassemblingHandler>());
        RegisterHandler(_container.Resolve<DamagableHandler>());
        RegisterHandler(_container.Resolve<HarvestHandler>());
        RegisterHandler(_container.Resolve<PickUpHandler>());
        RegisterHandler(_container.Resolve<ShowUIHandler>());
        RegisterHandler(_container.Resolve<DebugLabelHandlers>());

        foreach (var item in _handlerByModes)
        {
            var temp = item.Value.OrderByDescending(h => h.Priority).ToList();
            item.Value.Clear();
            item.Value.AddRange(temp);
        }
    }

    private void RegisterHandler(BaseInteractHandler handler)
    {
        _cachAllHandlers.Add(handler);
        _cacheCount = _cachAllHandlers.Count;
        if (!_handlerByModes.ContainsKey(handler.DefMode))
        {
            _handlerByModes.Add(handler.DefMode, new());
        }
        _handlerByModes[handler.DefMode].Add(handler);
    }

    private void OnHandle(InteractContext ctx)
    {
        if (!TryPerform(ctx, _handlerByModes[ctx.ModeInteract]))
        {
            Debug.LogWarning($"Not found handlers for: mode-{ctx.ModeInteract}, " +
                $"source-{ctx.Source?.DebugName ?? "none"}" +
                $"item-{ctx.Item?.DebugName ?? "none"}, " +
                $"target-{ctx.Target?.DebugName ?? "none"}");
        }
    }

    public List<BaseInteractHandler> GetValidCurrentFocus(ItemModel item, EntityModel targetEntity)
    {
        _validOnCurrentFocus.Clear();
        if(targetEntity == null)
            return _validOnCurrentFocus;

        for (int i = 0; i < _cacheCount; i++)
            if (_cachAllHandlers[i].CanHandle(item, targetEntity))
                _validOnCurrentFocus.Add(_cachAllHandlers[i]);

        return _validOnCurrentFocus;
    }

    public bool TryPerform(InteractContext ctx, List<BaseInteractHandler> handlers)
    {
        var count = handlers.Count;
        for (int i = 0; i < count; i++)
        {
            if (handlers[i].CanHandle(ctx.Item, ctx.Target) &&
                handlers[i].TryExecute(ctx.Source, ctx.Item, ctx.Target))
            {
                ExecutedTarget?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<InteractContext>(OnHandle);
    }
}