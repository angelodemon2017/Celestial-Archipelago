using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractCoordinator
{
    private Dictionary<EModeInteract, List<BaseInteractHandler>> _handlerByModes = new();

    public InteractCoordinator()
    {
        InitHandlers();
    }

    private void InitHandlers()
    {
        RegisterHandler(EModeInteract.LCM, new DamagableHandler());
        RegisterHandler(EModeInteract.LCM, new HarvestHandler());
        RegisterHandler(EModeInteract.EKB, new ShowUIHandler());

        foreach (var item in _handlerByModes)
        {
            var temp = item.Value.OrderBy(h => h.Priority).ToList();
            item.Value.Clear();
            item.Value.AddRange(temp);
        }
    }

    private void RegisterHandler(EModeInteract mode, BaseInteractHandler handler)
    {
        if (!_handlerByModes.ContainsKey(mode))
        {
            _handlerByModes.Add(mode, new());
        }
        _handlerByModes[mode].Add(handler);
    }

    public void PerformByMode(EModeInteract mode, InteractContext ctx)
    {
        if (!TryPerform(ctx, _handlerByModes[mode]))
        {
            Debug.LogWarning($"Not found handlers for: mode-{mode}, " +
                $"source-{ctx.Source?.DebugName ?? "none"}" +
                $"item-{ctx.Item?.DebugName ?? "none"}, " +
                $"target-{ctx.Target?.DebugName ?? "none"}");
        }
    }

    public bool TryPerform(InteractContext ctx, List<BaseInteractHandler> handlers)
    {
        foreach (var handler in handlers)
        {
            if (handler.CanHandle(ctx.Item, ctx.Target) &&
                handler.TryExecute(ctx.Source, ctx.Item, ctx.Target))
            {
                return true;
            }
        }
        return false;
    }
}