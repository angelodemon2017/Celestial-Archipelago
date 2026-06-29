
public class PickUpHandler : BaseInteractHandler
{
    public override int Priority => 1000;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return true;//check container of player Entity
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        return true;
    }
}