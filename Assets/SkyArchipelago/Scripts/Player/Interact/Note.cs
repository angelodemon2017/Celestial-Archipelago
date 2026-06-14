using UnityEngine;

public class Note : InteractableBase
{
    [Header("Note Settings")]
    [SerializeField, TextArea(5, 10)] private string _noteText = "Текст записки...";
    [SerializeField] private string _noteTitle = "Записка";

    public override bool TryInteract(out InteractionResult result)
    {
        result = new OpenNoteResult(_noteTitle, _noteText);
        return CanInteract;
    }
}