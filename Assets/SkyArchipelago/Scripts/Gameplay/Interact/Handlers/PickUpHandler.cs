using Zenject;

public class PickUpHandler : BaseInteractHandler
{
    private readonly SignalBus _signalBus;
    private readonly ItemsCatalogManager _itemsCatalogManager;
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;

    public override int Priority => 1000;
    public override EModeInteract DefMode => EModeInteract.EKB;

    public PickUpHandler(
        SignalBus signalBus,
        ItemsCatalogManager itemsCatalogManager,
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _signalBus = signalBus;
        _itemsCatalogManager = itemsCatalogManager;
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Item) == CtxFlag.Item;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is DroppedItemModel droppedItem))
            return false;

        if(!(source is IHaveContainer entityWithContainer))
            return false;

        var container = _containersService.GetContainerModel(entityWithContainer);
        if (_inventoryTransactionsService.TryAddItem(container, droppedItem.eItemType, droppedItem.Count, ContainerAvailabilityFlag.CanHandleDrop))
        {
            _signalBus.Fire(new EntityDeleteRequestSignal(target.Id, droppedItem.IdOwnedDrop));
        }

        return true;
    }

    public override string GetHint(EntityModel target)
    {
        if (target is DroppedItemModel droppedItem &&
            _itemsCatalogManager.TryGetConfigByKey(droppedItem.eItemType, out var itemConfig))
        {
            return $"{KeyPrefix} Подобрать {itemConfig.KeyName}({droppedItem.Count})";
        }
        else
        {
            return $"{KeyPrefix} Подобрать {target.ModelName}(?)";
        }
    }
}