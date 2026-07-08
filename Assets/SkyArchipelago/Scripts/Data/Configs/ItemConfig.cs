using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Config")]
public class ItemConfig : BaseDataConfig
{
    public EItemType TypeItem;
    public CtxFlag ItemTags = CtxFlag.Item;//TODO replace special tags for item?
    public string KeyName;
    public string KeyDesc;
    public int MaxStack;
    public float Weight;//??
}