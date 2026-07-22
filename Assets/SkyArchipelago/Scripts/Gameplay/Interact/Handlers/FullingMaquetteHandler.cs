using System;

public class FullingMaquetteHandler : BaseInteractHandler
{
    private readonly MaquetteReleaseService _maquetteReleaseService;
    private readonly EntityRecipeCatalogManager _entityRecipeCatalogManager;
    private readonly ContainersService _containersService;
    private readonly InventoryTransactionsService _inventoryTransactionsService;
    private readonly CraftingOperationService _craftingOperationService;

    public FullingMaquetteHandler(
        MaquetteReleaseService maquetteReleaseService,
        EntityRecipeCatalogManager entityRecipeCatalogManager,
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService,
        CraftingOperationService craftingOperationService)
    {
        _maquetteReleaseService = maquetteReleaseService;
        _entityRecipeCatalogManager = entityRecipeCatalogManager;
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
        _craftingOperationService = craftingOperationService;
    }

    public override EModeInteract DefMode => EModeInteract.EKB;
    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Maquette) == CtxFlag.Maquette;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is MaquetteOfEntityModel maquetteOfEntity))
            return false;

        if (!_entityRecipeCatalogManager.TryGetRecipeByEntityUId(maquetteOfEntity.GetIdEntity, out var recipe))
            return false;

        if (!(target is IHaveContainer haveContainer))
            return false;

        var containerMaquette = _containersService.GetContainerModel(haveContainer, EContainerType.MaquetteSource);

        if (_craftingOperationService.GetAvailableProductionsByRecipe(recipe, containerMaquette._dataModel) > 0)
        {
            _craftingOperationService.DeleteInputItems(containerMaquette, recipe);
            _maquetteReleaseService.ReleaseMaquette(target.Id);
            return true;
        }

        if (source is IHaveContainer containerOfSource)
        {
            var sourceContainer = _containersService.GetContainerModel(containerOfSource);
            var count = recipe._inputs.Count;
            for (int i = 0; i < count; i++)
            {
                var input = recipe._inputs[i];
                var haveOfReq = containerMaquette.GetItemCountByType(input.Config.TypeItem);
                if (haveOfReq < input.Amount)
                {
                    int needAmount = input.Amount - haveOfReq;
                    var haveInInventory = sourceContainer.GetItemCountByType(input.Config.TypeItem);
                    var workAmount = Math.Min(haveInInventory, needAmount);
                    if (_inventoryTransactionsService.TryRemoveItem(sourceContainer, input.Config, workAmount))
                        _inventoryTransactionsService.TryAddItem(containerMaquette, input.Config.TypeItem, workAmount, ContainerAvailabilityFlag.DropActions);
                }
            }
        }

        return true;
    }

    public override string GetHint(EntityModel target)
    {
        if (target is MaquetteOfEntityModel maquetteOfEntity &&
            _entityRecipeCatalogManager.TryGetRecipeByEntityUId(maquetteOfEntity.GetIdEntity, out var recipe) &&
            target is IHaveContainer haveContainer)
        {
            var container = _containersService.GetContainerModel(haveContainer, EContainerType.MaquetteSource);
            var count = recipe._inputs.Count;
            for (int i = 0; i < count; i++)
            {
                var input = recipe._inputs[i];
                var haveOfReq = container.GetItemCountByType(input.Config.TypeItem);
                if(haveOfReq < input.Amount)
                    return $"{KeyPrefix} Заполнить {target.ModelName}. {input.Config.KeyName}({haveOfReq}/{input.Amount})";
            }
        }

        return $"{KeyPrefix} Завершить {target.ModelName}";
    }
}