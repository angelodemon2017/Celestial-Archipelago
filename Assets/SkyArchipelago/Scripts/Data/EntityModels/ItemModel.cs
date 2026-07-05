using Zenject;

public class ItemModel : BaseModel<ItemData, ItemConfig>, IPoolable<ItemData>
{
    public EItemType TypeItem => _dataModel.TypeItem;
    public virtual CtxFlag GetTag => _dataModel.Config.ctxFlag;

    public string FullItemName => $"{_dataModel.SomePrefix}{_dataModel.Config.KeyName}";
    public string Description => _dataModel.Config.KeyDesc;
    public int Count => _dataModel.Amount;
    public override string ModelName => $"Some Item";

    public void OnSpawned(ItemData itemData)
    {
        _dataModel = itemData;
    }

    public void SetCount(int count)
    {
        _dataModel.Amount = count;

        if (_dataModel.Amount == 0)
        {
            _dataModel.TypeItem = EItemType.None;
        }
    }

    public void OnDespawned()
    {
        Changed = null;
    }
}