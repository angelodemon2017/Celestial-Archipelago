using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Config")]
public class ItemConfig : BaseDataConfig
{
    public EItemType TypeItem;
    public CtxFlag ctxFlag;
    public string KeyName;
    public string KeyDesc;
    public int MaxStack;
    public float Weight;//??
}