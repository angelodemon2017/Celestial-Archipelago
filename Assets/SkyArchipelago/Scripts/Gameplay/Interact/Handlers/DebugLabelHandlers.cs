using UnityEngine;

public class DebugLabelHandlers : BaseInteractHandler
{
    public override int Priority => 1;
    public override EModeInteract DefMode => EModeInteract.EKB;

    public DebugLabelHandlers()
    {

    }

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return target is DebugLabelModel;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is DebugLabelModel debugLabel))
            return false;

        Debug.Log($"DebugLabelHandlers: Interacted with DebugLabelModel: {debugLabel.DebugText}");

        return true;
    }

    public override string GetHint(EntityModel target)
    {
        return $"{KeyPrefix} Выполнить {target.ModelName}";
    }
}