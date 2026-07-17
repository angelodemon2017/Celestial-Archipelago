using System;
using System.Collections.Generic;
using Zenject;

public class RuntimeBurningsHandlerService : ITimeTickable, IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly SystemSO _systemSO;
    private readonly IGameTimeService _timeService;
    private readonly BurningFuelsRepository _burningFuelsRepository;
    private readonly ContainersService _containersService;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly InventoryTransactionsService _inventoryTransactionsService;

    private float _burningTick;
    private Queue<BurningFuelData> _queueForChecking = new();
    private int _sizeQueue = 0;

    public RuntimeBurningsHandlerService(
        SignalBus signalBus,
        SystemSO systemSO,
        IGameTimeService timeService,
        BurningFuelsRepository burningFuelsRepository,
        ContainersService containersService,
        EntityRuntimeService entityRuntimeService,
        InventoryTransactionsService inventoryTransactionsService)
    {
        _signalBus = signalBus;
        _systemSO = systemSO;
        _timeService = timeService;
        _burningFuelsRepository = burningFuelsRepository;
        _containersService = containersService;
        _entityRuntimeService = entityRuntimeService;
        _inventoryTransactionsService = inventoryTransactionsService;
    }

    public void Initialize()
    {
        _timeService.Register(this);
        _signalBus.Subscribe<ContainerOfEntityRequest>(OnHandle);
    }

    public void Dispose()
    {
        _timeService.Unregister(this);
        _signalBus?.Unsubscribe<ContainerOfEntityRequest>(OnHandle);
    }

    private void OnHandle(ContainerOfEntityRequest containerOfEntityRequest)
    {
        if (_entityRuntimeService.TryGetEntityById(containerOfEntityRequest.IdEntity, out var entity))
        {
            if (entity is IBurnable burnable)
                CheckFueling(burnable);
        }
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _burningTick += gameDeltaTime;

        while (_burningTick > _systemSO.globalBurnPeriod)
        {
            _burningTick -= _systemSO.globalBurnPeriod;
            BurnTick();
        }
    }

    private void BurnTick()
    {
        var list = _burningFuelsRepository.GetAllBurns;
        for (int i = 0; i < _burningFuelsRepository.CountBurns; i++)
        {
            var burn = list[i];
            if (burn.Storage > 0)
            {
                burn.Storage--;
                if (_entityRuntimeService.TryGetEntityById(burn.TargetEntityId, out var entity) &&
                    entity is IBurnable burnable)//TODO maybe need faster
                    burnable.HaveChange = true;
            }
            if (burn.Storage <= 0)
            {
                _queueForChecking.Enqueue(burn);
                _sizeQueue++;
            }
        }
        while (_sizeQueue > 0)
        {
            Recheck(_queueForChecking.Dequeue());
            _sizeQueue--;
        }
    }

    private void Recheck(BurningFuelData fuelData)
    {
        var entityId = fuelData.TargetEntityId;
        _burningFuelsRepository.RemoveBurn(fuelData.Id);
        if (_entityRuntimeService.TryGetEntityById(entityId, out var entity) &&
            entity is IBurnable burnable)
        {
            burnable.BurnIdProcess = -1;
            CheckFueling(burnable);
        }
    }

    private void CheckFueling(IBurnable burnableEntity)
    {
        if (_burningFuelsRepository.TryGetBurnById(burnableEntity.BurnIdProcess, out var burn))
            return;

        if (burnableEntity.IsNeedBurn &&
            burnableEntity is IHaveContainer haveContainer)
        {
            var container = _containersService.GetContainerModel(haveContainer, EContainerType.BurningFuel);
            if (container.IsEmpty)
                return;

            for (int i = 0; i < container.itemModels.Count; i++)
            {
                var item = container.itemModels[i];
                if ((item.ItemTags & CtxFlag.Burning) == CtxFlag.Burning &&
                    item.Count > 0)
                {
                    var itemConfig = item.ConfigModel;
                    if(_inventoryTransactionsService.TryRemoveItem(container, item.ConfigModel, 1))
                        _burningFuelsRepository.StartBurnProcess(itemConfig, burnableEntity);
                }
            }
        }
    }
}