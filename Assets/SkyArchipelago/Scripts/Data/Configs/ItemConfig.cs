using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemConfig : ScriptableObject
{
    public EItemType TypeItem;
    public string KeyName;
    public string KeyDesc;
    public int MaxStack;
}