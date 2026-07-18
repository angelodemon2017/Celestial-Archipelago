using System;
using Zenject;

public class CraftingOperationService
{
    private readonly SignalBus _signalBus;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly InventoryTransactionsService _inventoryTransactionsService;

    public CraftingOperationService(
        SignalBus signalBus,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        ItemModelFactory itemModelFactory,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _signalBus = signalBus;
        _itemDataFactory = itemDataFactory;
        _itemModelFactory = itemModelFactory;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public int GetAvailableProductionByRecipe(RecipeConfig recipeConfig, ContainerData container)
    {
        int haveItemsForRecipe = int.MaxValue;

        for (int i = 0; i < recipeConfig._inputs.Count && haveItemsForRecipe > 0; i++)
        {
            var itemAmount = recipeConfig._inputs[i];
            int amountInContainer = container.GetItemCountByType(itemAmount.Config.TypeItem);
            int result = amountInContainer / itemAmount.Amount;
            haveItemsForRecipe = Math.Min(haveItemsForRecipe, result);
        }

        return haveItemsForRecipe;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>True - is done craft</returns>
    public bool TakeUA(CraftProcessModel craftProcess, int amountUA)
    {
        if (!craftProcess.craftable.IsActive)
            return false;

        if (craftProcess.ConfigModel.IsDeleteInputsOnStart &&
            craftProcess.Process == 0)
            DeleteInputItems(craftProcess.SourceContainer, craftProcess.ConfigModel);

        var totalUA = craftProcess.Process += amountUA;
        if (totalUA >= craftProcess.ConfigModel.ActionUnitsRequired)
        {
            craftProcess.Process -= craftProcess.ConfigModel.ActionUnitsRequired;

            if (!craftProcess.ConfigModel.IsDeleteInputsOnStart &&
                GetAvailableProductionByRecipe(craftProcess.ConfigModel, craftProcess.SourceContainer._dataModel) <= 0)
                return false;

            if (!IsAvailableContainer(craftProcess.ProductionContainer, craftProcess.ConfigModel))
                return false;

            AddProduction(craftProcess.ProductionContainer, craftProcess.ConfigModel);

            if (!craftProcess.ConfigModel.IsDeleteInputsOnStart)
                DeleteInputItems(craftProcess.SourceContainer, craftProcess.ConfigModel);
            _signalBus.Fire(new ContainerOfEntityRequest(craftProcess.ProductionContainer.IdEntityOwner,
                craftProcess.ProductionContainer.Id));
            return true;
        }
        return false;
    }

    private bool IsAvailableContainer(ContainerModel productionContainer, RecipeConfig recipe)
    {
        var outputs = recipe._outputs;
        var countOutputs = outputs.Count;
        var needEmptySlots = 0;
        for (int i = 0; i < countOutputs; i++)
        {
            var outp = outputs[i];
            var availableSpace = productionContainer._dataModel.GetAvailableSpaceByType(outp.Config.TypeItem);
            if (availableSpace < outp.Amount)
                needEmptySlots++;
        }
        var emptySlots = productionContainer._dataModel.EmptySlots;
        return needEmptySlots <= emptySlots;
    }

    private void DeleteInputItems(ContainerModel sourceContainer, RecipeConfig recipe)
    {
        var countInputs = recipe._inputs.Count;
        for (int i = 0; i < countInputs; i++)
        {
            var inp = recipe._inputs[i];
            _inventoryTransactionsService.TryRemoveItem(sourceContainer, inp.Config, inp.Amount);
        }
        _signalBus.Fire(new ContainerOfEntityRequest(sourceContainer.IdEntityOwner, sourceContainer.Id));
    }

    private void AddProduction(ContainerModel productionContainer, RecipeConfig recipe)
    {
        int countOutputs = recipe._outputs.Count;
        for (int i = 0; i < countOutputs; i++)
        {
            var outp = recipe._outputs[i];
            var newItemData = _itemDataFactory.Spawn(outp.Config);
            newItemData.Amount = outp.Amount;
            var newItemModel = _itemModelFactory.Spawn(newItemData);
            var reason = recipe.IsCalcingInAutoCraftTicks ? ContainerAvailabilityFlag.CanAutoDrop : ContainerAvailabilityFlag.CanHandleDrop;
            _inventoryTransactionsService.TryAddItemModel(productionContainer, newItemModel, reason);
        }
    }
}