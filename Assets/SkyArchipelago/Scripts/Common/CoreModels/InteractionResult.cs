using UnityEngine;

public interface InteractionResult { }

public struct OpenDialogueResult : InteractionResult
{
    public string NpcId { get; }

    public OpenDialogueResult(string npcId)
    {
        NpcId = npcId;
    }
}

public struct PickupResult : InteractionResult
{
    public GameObject Go { get; }
    public ItemData Item { get; }
    public int Quantity { get; }

    public PickupResult(GameObject go, ItemData item, int quantity = 1)
    {
        Go = go;
        Item = item;
        Quantity = quantity;
    }
}

public struct OpenUIResult : InteractionResult
{
    public string UiType { get; }      // "Workbench", "CraftingTable", "Chest" и т.д.
    public object Data { get; }

    public OpenUIResult(string uiType, object data = null)
    {
        UiType = uiType;
        Data = data;
    }
}

public struct OpenNoteResult : InteractionResult
{
    public string Title { get; }
    public string Text { get; }

    public OpenNoteResult(string title, string text)
    {
        Title = title;
        Text = text;
    }
}