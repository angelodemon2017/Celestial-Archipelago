public class PickUpHandler : BaseInteractHandler
{
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;

    public PickUpHandler(
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public override int Priority => 1000;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return target is DroppedItemModel;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is DroppedItemModel droppedItem))
            return false;

        if(!(source is IHaveContainer entityWithContainer))
            return false;

        var container = _containersService.GetContainerModel(entityWithContainer);
        if (_inventoryTransactionsService.TryAddItemToContainer(container, droppedItem.CurrentItem))
        {
            container?.Changed();
            droppedItem?.Changed();
        }

        return true;
    }
}