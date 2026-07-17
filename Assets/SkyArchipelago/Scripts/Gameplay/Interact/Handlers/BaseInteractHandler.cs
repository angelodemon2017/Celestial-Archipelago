public abstract class BaseInteractHandler
{
    public abstract int Priority { get; }
    /// <summary>
    /// Fast validation by tags
    /// </summary>
    /// <param name="item"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public abstract bool CanHandle(ItemModel item, EntityModel target);
    public abstract bool TryExecute(EntityModel source, ItemModel item, EntityModel target);
}