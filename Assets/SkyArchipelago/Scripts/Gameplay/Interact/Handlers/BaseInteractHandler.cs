public abstract class BaseInteractHandler
{
    public abstract int Priority { get; }
    public abstract EModeInteract DefMode { get; }
    public abstract string GetHint(EntityModel target);
    protected string KeyPrefix
    {
        get
        {
            switch (DefMode)
            {
                case EModeInteract.LCM:
                    return "(L Mouse)";
                case EModeInteract.RCM:
                    return "(R Mouse)";
                case EModeInteract.EKB:
                    return "(E Key)";
                default:
                    return string.Empty;
            }
        }
    }
    /// <summary>
    /// Fast validation by tags
    /// </summary>
    /// <param name="item"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CanHandle(ItemModel item, EntityModel target);
    public abstract bool TryExecute(EntityModel source, ItemModel item, EntityModel target);
}