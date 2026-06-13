using UnityEngine;

public abstract class NPCBase : InteractableBase
{
    [Header("NPC Settings")]
    [SerializeField] private string _npcName = "NPC";

    public string NpcName => _npcName;

    public override void OnFocusEnter()
    {
        base.OnFocusEnter();
        // Можно показать имя над головой и т.д.
    }

    public override void Interact()
    {
        if (!CanInteract) return;

        Debug.Log($"Разговор с {_npcName}");

        // Открыть UI диалога
        // DialogueUI.Instance.OpenDialogue(_dialogueData, this);
    }

    // Для квестов, торговли и т.д.
    public virtual void StartTrade() { }
    public virtual void GiveQuest() { }
}