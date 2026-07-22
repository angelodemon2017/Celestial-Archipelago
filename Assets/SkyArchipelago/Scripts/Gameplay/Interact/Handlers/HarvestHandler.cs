using UnityEngine;

public class HarvestHandler : BaseInteractHandler
{
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly ContainersService _containersService;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;

    public HarvestHandler(
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService,
        EntitiesCatalogManager entitiesCatalogManager)
    {
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
        _entitiesCatalogManager = entitiesCatalogManager;
    }

    public override int Priority => 25;
    public override EModeInteract DefMode => EModeInteract.RCM;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Harvesting) == CtxFlag.Harvesting;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(source is IHaveContainer haveContainer))
            return false;

        if (!(_entitiesCatalogManager.TryGetModule(target.ConfigId, CtxFlag.Harvesting, out var module) &&
            module is HarvestConfig harvestConfig))
            return false;

        //release check availabling items with expand items content
        //        if(!harvestable.AvailableHarvestBy(item))
        //            return false;

        var outputVar = harvestConfig._outputs.GetRandom();

        var container = _containersService.GetContainerModel(haveContainer);
        if (_inventoryTransactionsService.TryAddItem(container, outputVar.Config.TypeItem, outputVar.Amount, ContainerAvailabilityFlag.CanHandleDrop))
        {
            Debug.Log($"Harvested {outputVar.Config.KeyName} {outputVar.Amount}");
        }
        return true;
    }

    public override string GetHint(EntityModel target)
    {
        return $"{KeyPrefix} Добывать {target.ModelName}";
    }
}