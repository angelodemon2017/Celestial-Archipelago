using Zenject;

public class PickUpHandler : BaseInteractHandler
{
    private readonly SignalBus _signalBus;
    private readonly ItemsCatalogManager _itemsCatalogManager;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;

    public PickUpHandler(
        SignalBus signalBus,
        ItemsCatalogManager itemsCatalogManager,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        ItemModelFactory itemModelFactory,
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _signalBus = signalBus;
        _itemsCatalogManager = itemsCatalogManager;
        _itemDataFactory = itemDataFactory;
        _itemModelFactory = itemModelFactory;
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public override int Priority => 1000;

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

        if (!_itemsCatalogManager.TryGetConfigByKey(droppedItem.eItemType, out var itemConfig))
            return false;

        var itemData = _itemDataFactory.Spawn(itemConfig);
        itemData.Amount = droppedItem.Count;
        var itemModel = _itemModelFactory.Spawn(itemData);

        var container = _containersService.GetContainerModel(entityWithContainer);
        if (_inventoryTransactionsService.TryPickItemToContainer(container, itemModel))
        {
            _signalBus.Fire(new EntityDeleteRequestSignal(target.Id, droppedItem.IdOwnedDrop));
        }
        _itemDataFactory.Despawn(itemData);
        _itemModelFactory.Despawn(itemModel);

        return true;
    }
}