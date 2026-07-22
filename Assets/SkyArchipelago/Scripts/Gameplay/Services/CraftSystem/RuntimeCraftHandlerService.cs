using System;
using System.Collections.Generic;
using Zenject;

public class RuntimeCraftHandlerService : ITimeTickable, IInitializable, IDisposable
{
    private readonly SystemSO _systemSO;
    private readonly IGameTimeService _timeService;
    private readonly CraftProcessRepository _craftProcessRepository;
    private readonly CraftingOperationService _craftingOperationService;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly InventoryTransactionsService _inventoryTransactionsService;

    private Queue<CraftProcessModel> _craftsForRecalc = new();
    private int _sizeQueue = 0;

    private float _autoCraftTick;

    public RuntimeCraftHandlerService(
        SystemSO systemSO,
        IGameTimeService timeService,
        CraftProcessRepository craftProcessRepository,
        CraftingOperationService craftingOperationService,
        ItemModelFactory itemModelFactory,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _systemSO = systemSO;
        _timeService = timeService;
        _craftProcessRepository = craftProcessRepository;
        _craftingOperationService = craftingOperationService;
        _itemModelFactory = itemModelFactory;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _autoCraftTick += gameDeltaTime;

        while (_autoCraftTick > _systemSO.globalAutoCraftAction)
        {
            _autoCraftTick -= _systemSO.globalAutoCraftAction;
            RunTickAutocraft();
        }
    }

    private void RunTickAutocraft()
    {
        var listCrafts = _craftProcessRepository.GetAllCraft;
        for (int i = 0; i < _craftProcessRepository.CountCrafts; i++)
        {
            var craft = listCrafts[i];
            var uaPower = craft.craftable.UAPower;
            var isStaticRecipe = craft.ConfigModel.IsStaticRecipe;//todo move logic to other place
            if (_craftingOperationService.TakeUA(craft, uaPower))
            {

            }
        }
    }

    public void Initialize()
    {
        _timeService.Register(this);
    }

    public void Dispose()
    {
        _timeService.Unregister(this);
    }
}