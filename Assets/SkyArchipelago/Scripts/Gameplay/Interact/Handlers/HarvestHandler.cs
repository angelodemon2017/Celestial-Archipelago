using UnityEngine;

public class HarvestHandler : BaseInteractHandler
{
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemsCatalogConfig _itemsCatalogConfig;

    public HarvestHandler(
        ItemsCatalogConfig itemsCatalogConfig,
        ContainersService containersService,
        ItemModelFactory itemModelFactory,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _itemsCatalogConfig = itemsCatalogConfig;
        _containersService = containersService;
        _itemModelFactory = itemModelFactory;
        _itemDataFactory = itemDataFactory;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        if (!(target is IHarvestable harvestable))
            return false;

        return harvestable.AvailableHarvestBy(item);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(source is IHaveContainer haveContainer))
            return false;

        if(!(target is IHarvestable harvestable))
            return false;

        EItemType randType = harvestable.GetHarvestableItemType();
        var itemConfig = _itemsCatalogConfig.GetItemConfig(randType);
        var newDataItem = _itemDataFactory.Create(itemConfig);
        newDataItem.Amount = harvestable.GetHarvestableCount();
        var newModelItem = _itemModelFactory.Spawn(newDataItem);

        var container = _containersService.GetContainerModel(haveContainer);
        if (_inventoryTransactionsService.TryAddItemToContainer(container, newModelItem))
        {
            Debug.Log($"Harvested {itemConfig.KeyName} {newDataItem.Amount}");
            container?.Changed?.Invoke();
        }

        return true;
    }
}