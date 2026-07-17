using System.Collections.Generic;

public class BurningFuelsRepository
{
    private readonly DataService _dataService;
    private readonly SimpleFactory<ItemConfig, BurningFuelData> _burningsFactory;

    private Dictionary<int, BurningFuelData> _mapOfProcesses = new();
    private List<BurningFuelData> _activBurns = new();
    private int _countBurns = 0;

    public List<BurningFuelData> GetAllBurns => _activBurns;
    public int CountBurns => _countBurns;

    public BurningFuelsRepository(
        DataService dataService,
        SimpleFactory<ItemConfig, BurningFuelData> burningsFactory)
    {
        _dataService = dataService;
        _burningsFactory = burningsFactory;
    }

    public BurningFuelData StartBurnProcess(
        ItemConfig itemConfig,
        IBurnable targetEntity)
    {
        var burn = _burningsFactory.Spawn(itemConfig);
        burn.Id = _dataService.AddNewBurn(burn);
        burn.TargetEntityId = targetEntity.Id;
        targetEntity.BurnIdProcess = burn.Id;
        AddToCollections(burn);
        return burn;
    }

    public bool TryGetBurnById(int id, out BurningFuelData craftProcess)
    {
        if (!_mapOfProcesses.ContainsKey(id) &&
            _dataService.TryGetBurn(id, out var craft))
        {
            AddToCollections(craft);
        }
        return _mapOfProcesses.TryGetValue(id, out craftProcess);
    }

    private void AddToCollections(BurningFuelData burn)
    {
        _mapOfProcesses[burn.Id] = burn;
        _activBurns.Add(burn);
        _countBurns++;
    }

    public void RemoveBurn(int id)
    {
        if (_mapOfProcesses.TryGetValue(id, out var craft))
        {
            _dataService.RemoveBurn(id);
            _mapOfProcesses.Remove(id);
            _countBurns--;
            _activBurns.Remove(craft);
            _burningsFactory.Despawn(craft);
        }
    }
}