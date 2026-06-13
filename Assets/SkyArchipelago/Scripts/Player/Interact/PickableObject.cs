using UnityEngine;

public class PickableObject : InteractableBase
{
    [Header("Pickable Settings")]
    [SerializeField] private ItemData _itemData;           // ScriptableObject с данными предмета
    [SerializeField] private int _quantity = 1;

    public override void Interact()
    {
        if (!CanInteract) return;

        // Здесь логика подбора
        Debug.Log($"Подобрано: {_itemData?.ItemName} x{_quantity}");

        // Пример: добавить в инвентарь
        // InventoryService.Instance.AddItem(_itemData, _quantity);

        Destroy(gameObject); // или отключить
    }

    public ItemData ItemData => _itemData;
    public int Quantity => _quantity;
}