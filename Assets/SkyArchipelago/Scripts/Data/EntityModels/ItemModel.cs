using Zenject;

public class ItemModel : BaseModel<ItemData, ItemConfig>, IPoolable<ItemData>
{
    public int SlotId => _dataModel.SlotId;
    public EItemType TypeItem => _dataModel.TypeItem;
    public CtxFlag ItemTags => _dataModel.Config.ItemTags;
    public virtual ERarityItem RarityItem => ERarityItem.Normal;

    public virtual bool IsUsable => false;
    public virtual string FullItemName => $"{_dataModel.SomePrefix}{_dataModel.Config.KeyName}";
    public string Description => _dataModel.Config.KeyDesc;
    public int Count => _dataModel.Amount;
    public int MaxStack => ConfigModel.MaxStack;
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
        _dataModel = null;
        Changed = null;
    }
}