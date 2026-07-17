using System.Collections.Generic;

public class CraftProcessRepository
{
    private readonly DataService _dataService;
    private readonly SimpleFactory<RecipeConfig, CraftProcessData> _craftProcDataFactory;
    private readonly SimpleFactory<CraftProcessData, CraftProcessModel> _craftProcModelFactory;
    private readonly ContainersService _containersService;

    private Dictionary<int, CraftProcessModel> _mapOfProcesses = new();
    private List<CraftProcessModel> _activCrafts = new();
    private int _countCrafts = 0;

    public List<CraftProcessModel> GetAllCraft => _activCrafts;
    public int CountCrafts => _countCrafts;

    public CraftProcessRepository(
        DataService dataService,
        SimpleFactory<RecipeConfig, CraftProcessData> craftProcDataFactory,
        SimpleFactory<CraftProcessData, CraftProcessModel> craftProcModelFactory,
        ContainersService containersService)
    {
        _dataService = dataService;
        _craftProcDataFactory = craftProcDataFactory;
        _craftProcModelFactory = craftProcModelFactory;
        _containersService = containersService;
    }

    public CraftProcessModel StartCraft(
        IEntity reasonEntity,
        ICraftable targetEntity,
        RecipeConfig recipe)
    {
        CraftProcessData craftProcessData = _craftProcDataFactory.Spawn(recipe);
        craftProcessData.RecipeId = recipe.Uid;
        craftProcessData.TargetEntityId = targetEntity.Id;
        ContainerModel containerSource = null;
        ContainerModel containerProduction = null;
        if ((targetEntity.AvailableTags & CtxFlag.HaveContainers) == CtxFlag.HaveContainers &&
            targetEntity is IHaveContainer haveContainer)
        {
            containerSource = _containersService.GetContainerModel(haveContainer, EContainerType.SourceInput);
            craftProcessData.SourceContainerId = containerSource.Id;
            containerProduction = _containersService.GetContainerModel(haveContainer, EContainerType.ProductionOutput);
            craftProcessData.ProductionContainerId = containerProduction.Id;
        }
        else if (reasonEntity is IHaveContainer reasonContainer)
        {
            containerSource = _containersService.GetContainerModel(reasonContainer);
            craftProcessData.SourceContainerId = containerSource.Id;
            craftProcessData.ProductionContainerId = containerSource.Id;
        }

        craftProcessData.Id = _dataService.AddNewCraft(craftProcessData);

        return GetCraftModel(craftProcessData,
            targetEntity,
            containerSource,
            containerProduction);
    }

    public CraftProcessModel GetCraftModel(
        CraftProcessData craftProcessData,
        ICraftable craftable = null,
        ContainerModel sourceContainer = null,
        ContainerModel productionContainer = null)
    {
        var newCraftModel = _craftProcModelFactory.Spawn(craftProcessData);
        newCraftModel.craftable = craftable;//??
        newCraftModel.SourceContainer = sourceContainer ?? _containersService.GetContainerModelById(newCraftModel._dataModel.SourceContainerId);
        newCraftModel.ProductionContainer = productionContainer ?? _containersService.GetContainerModelById(newCraftModel._dataModel.ProductionContainerId);
        if (newCraftModel.ConfigModel.IsCalcingInAutoCraftTicks)
        {
            _activCrafts.Add(newCraftModel);
            _countCrafts++;
        }
        _mapOfProcesses[newCraftModel.Id] = newCraftModel;
        return newCraftModel;
    }

    public bool TryGetCraftById(int id, out CraftProcessModel craftProcess)
    {
/*        if (!_mapOfProcesses.ContainsKey(id) &&
            _dataService.TryGetCraft(id, out var craft))
        {
            GetCraftModel(craft);
        }/**/
        return _mapOfProcesses.TryGetValue(id, out craftProcess);
    }

    public void RemoveCraft(int id)
    {
        if (_mapOfProcesses.TryGetValue(id, out var craft))
        {
            _dataService.RemoveCraft(id);
            _mapOfProcesses.Remove(id);
            if (craft.ConfigModel.IsCalcingInAutoCraftTicks)
            {
                _countCrafts--;
                _activCrafts.Remove(craft);
            }
            _craftProcDataFactory.Despawn(craft._dataModel);
            _craftProcModelFactory.Despawn(craft);
        }
    }
}