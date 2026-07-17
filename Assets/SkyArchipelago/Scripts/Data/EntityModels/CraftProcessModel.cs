using Zenject;

public class CraftProcessModel : BaseModel<CraftProcessData, RecipeConfig>, IPoolable<CraftProcessData>
{
    public int Process
    {
        get => _dataModel.Process;
        set
        {
            if (_dataModel.Process != value)
                craftable.HaveChange = true;
            _dataModel.Process = value;
        }
    }
    public override string ModelName => ConfigModel._outputs[0].Config.KeyName;
    public ICraftable craftable;
    public ContainerModel SourceContainer;
    public ContainerModel ProductionContainer;

    public void OnSpawned(CraftProcessData itemData)
    {
        _dataModel = itemData;
    }

    public void OnDespawned()
    {
        _dataModel = null;
    }
}