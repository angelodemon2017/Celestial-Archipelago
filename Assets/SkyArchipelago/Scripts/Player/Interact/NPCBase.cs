using UnityEngine;

public abstract class NPCBase : InteractableBase
{
    [Header("NPC Settings")]
    [SerializeField] protected string _npcName = "NPC";

    public string NpcName => _npcName;

    public override void OnFocusEnter()
    {
        base.OnFocusEnter();
        // Можно показать имя над головой и т.д.
    }

    // Для квестов, торговли и т.д.
    public virtual void StartTrade() { }
    public virtual void GiveQuest() { }
}