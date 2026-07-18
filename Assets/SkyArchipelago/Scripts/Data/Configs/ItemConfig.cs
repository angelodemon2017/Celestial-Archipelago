using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Config")]
public class ItemConfig : BaseDataConfig, BaseCatalogElementConfig<EItemType>
{
    //public short Uid;//??replace TypeItem?
    public EItemType TypeItem;
    public CtxFlag ItemTags = CtxFlag.Item;//TODO replace special tags for item?
    public string KeyName;
    public string KeyDesc;
    public int MaxStack;
    public int FuelStorage = 0;
    public GameObject PrefabOfItem;//TODO model of building

    public EItemType KeyOfCatalog
    {
        get => TypeItem;
        set => TypeItem = value;
    }
}