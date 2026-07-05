public class InventoryTransactionsService
{
    private readonly DataService _dataService;
    private readonly ItemModelFactory _itemModelFactory;

    public InventoryTransactionsService(
        ItemModelFactory itemModelFactory,
        DataService dataService)
    {
        _itemModelFactory = itemModelFactory;
        _dataService = dataService;
    }

    public bool TryAddItemToContainer(ContainerModel containerModel, ItemModel itemModel)
    {
        if(!containerModel.AvailableSlots(itemModel))
            return false;

        var leftCount = containerModel.AddItem(itemModel);
        if (leftCount > 0)
        {
            _itemModelFactory.Despawn(itemModel);
        }

        return true;
    }

    public void MoveItem(ContainerModel fromContainer, ContainerModel toContainer, ItemModel itemModel)
    {

    }
}