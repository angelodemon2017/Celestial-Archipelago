
public class HarvestHandler : BaseInteractHandler
{
    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (item.GetTag & target.AvailableFlag)
            .HasFlag(CtxFlag.Harvesting);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        return true;
    }
}