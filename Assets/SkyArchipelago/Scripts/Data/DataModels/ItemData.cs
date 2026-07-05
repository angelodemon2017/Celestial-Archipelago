using Zenject;

[System.Serializable]
public class ItemData : BaseData<ItemConfig>, IPoolable<ItemConfig>
{
    public EItemType TypeItem;
    public int Amount;
    public string SomePrefix;//enchant

    public void OnSpawned(ItemConfig memoryPool)
    {
        InitConfig(memoryPool);
    }

    public override void InitConfig(ItemConfig config)
    {
        base.InitConfig(config);
        EntityType = EEntityType.Item;
        TypeItem = config.TypeItem;
    }

    public void OnDespawned()
    {
        SomePrefix = string.Empty;
    }
}