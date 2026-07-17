using UnityEngine;

public class HarvestHandler : BaseInteractHandler
{
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemsCatalogManager _itemsCatalogManager;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;

    public HarvestHandler(
        ContainersService containersService,
        ItemsCatalogManager itemsCatalogManager,
        ItemModelFactory itemModelFactory,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        InventoryTransactionsService inventoryTransactionsService,
        EntitiesCatalogManager entitiesCatalogManager)
    {
        _containersService = containersService;
        _itemsCatalogManager = itemsCatalogManager;
        _itemModelFactory = itemModelFactory;
        _itemDataFactory = itemDataFactory;
        _inventoryTransactionsService = inventoryTransactionsService;
        _entitiesCatalogManager = entitiesCatalogManager;
    }

    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Harvesting) == CtxFlag.Harvesting;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(source is IHaveContainer haveContainer))
            return false;

        if (!(_entitiesCatalogManager.TryGetModule(target.EntType, CtxFlag.Harvesting, out var module) &&
            module is HarvestConfig harvestConfig))
            return false;

        if (!(target is IHarvestable harvestable))
            return false;

        //release check availabling items with expand items content
        //        if(!harvestable.AvailableHarvestBy(item))
        //            return false;

        EItemType randType = harvestable.GetHarvestableItemType();
        if (!_itemsCatalogManager.TryGetConfigByKey(randType, out var itemConfig))
            return false;
        var newDataItem = _itemDataFactory.Spawn(itemConfig);
        int totalAmount = harvestable.GetHarvestableCount();
        newDataItem.Amount = totalAmount;
        var newModelItem = _itemModelFactory.Spawn(newDataItem);

        var container = _containersService.GetContainerModel(haveContainer);
        if (_inventoryTransactionsService.TryPickItemToContainer(container, newModelItem))
        {
            Debug.Log($"Harvested {itemConfig.KeyName} {totalAmount}");
        }
        _itemDataFactory.Despawn(newModelItem._dataModel);
        _itemModelFactory.Despawn(newModelItem);

        return true;
    }
}