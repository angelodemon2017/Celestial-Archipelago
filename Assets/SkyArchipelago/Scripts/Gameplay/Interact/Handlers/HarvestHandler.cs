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
        return target.AvailableTags.HasFlag(CtxFlag.Harvesting);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(source is IHaveContainer haveContainer))
            return false;

        if(!(target is IHarvestable harvestable))
            return false;

//release check availabling items with expand items content
//        if(!harvestable.AvailableHarvestBy(item))
//            return false;

        EItemType randType = harvestable.GetHarvestableItemType();
        var itemConfig = _itemsCatalogConfig.GetItemConfig(randType);
        var newDataItem = _itemDataFactory.Create(itemConfig);
        int totalAmount = harvestable.GetHarvestableCount();
        newDataItem.Amount = totalAmount;
        var newModelItem = _itemModelFactory.Spawn(newDataItem);

        var container = _containersService.GetContainerModel(haveContainer);
        if (_inventoryTransactionsService.TryPickItemToContainer(container, newModelItem))
        {
            Debug.Log($"Harvested {itemConfig.KeyName} {totalAmount}");
            container?.Changed?.Invoke();
        }

        return true;
    }
}