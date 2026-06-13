using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public string Description;
    // weight, rarity и т.д.
}