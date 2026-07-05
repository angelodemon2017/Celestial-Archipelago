public class DroppedItemModel : EntityModel<DroppedItemData>
{
    public ItemModel CurrentItem;

    public DroppedItemModel(DroppedItemData data) : base(data)
    {
    }
}