using UnityEngine;

public class PickableObject : InteractableBase
{
    [Header("Pickable Settings")]
    [SerializeField] private ItemConfig _itemData;           // ScriptableObject с данными предмета
    [SerializeField] private int _quantity = 1;

    public override bool TryInteract(out InteractionResult result)
    {
        result = 
            new PickupResult(
                gameObject,
                _itemData,
                _quantity);

        return CanInteract;
    }

    public ItemConfig ItemData => _itemData;
    public int Quantity => _quantity;
}