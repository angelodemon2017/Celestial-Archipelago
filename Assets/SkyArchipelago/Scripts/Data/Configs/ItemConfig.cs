using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemConfig : BaseDataConfig
{
    public EItemType TypeItem;
    public CtxFlag ctxFlag;
    public string KeyName;
    public string KeyDesc;
    public int MaxStack;
    public float Weight;//??
}