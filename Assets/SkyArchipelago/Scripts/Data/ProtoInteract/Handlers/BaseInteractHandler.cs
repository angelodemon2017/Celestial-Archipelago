public abstract class BaseInteractHandler
{
    public abstract int Priority { get; }
    public abstract bool CanHandle(ItemModel item, EntityModel target);
    public abstract bool TryExecute(EntityModel source, ItemModel item, EntityModel target);
}