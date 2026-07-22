using Zenject;

public class DisassemblingHandler : BaseInteractHandler
{
    private readonly SignalBus _signalBus;
    private readonly EntityRecipeCatalogManager _recipeCatalogManager;
    private readonly ContainersService _containersService;
    private readonly InventoryTransactionsService _inventoryTransactionsService;

    public DisassemblingHandler(
        SignalBus signalBus,
        EntityRecipeCatalogManager recipeCatalogManager,
        ContainersService containersService,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _signalBus = signalBus;
        _recipeCatalogManager = recipeCatalogManager;
        _containersService = containersService;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public override int Priority => 25;
    public override EModeInteract DefMode => EModeInteract.RCM;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Disassemble) == CtxFlag.Disassemble;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
//        if(!(target is MaquetteOfEntityModel moem))
//            return false;

        if (source is IHaveContainer haveContainer &&
            _recipeCatalogManager.TryGetRecipeByEntityUId(target.ConfigId, out var recipeEntity))
        {
            var container = _containersService.GetContainerModel(haveContainer);
            var count = recipeEntity._inputs.Count;
            for (int i = 0; i < count; i++)
            {
                var input = recipeEntity._inputs[i];
                _inventoryTransactionsService.TryAddItem(container, input.Config.TypeItem, input.Amount, ContainerAvailabilityFlag.CanHandleDrop);
            }
        }        

        _signalBus.Fire(new EntityDeleteRequestSignal(target.Id));
        return true;
    }

    public override string GetHint(EntityModel target)
    {
        return $"{KeyPrefix} Разобрать {target.ModelName}";
    }
}