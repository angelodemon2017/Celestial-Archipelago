using UnityEngine;

public class Note : InteractableBase
{
    [Header("Note Settings")]
    [SerializeField, TextArea(5, 10)] private string _noteText = "Текст записки...";
    [SerializeField] private string _noteTitle = "Записка";

    public string NoteText => _noteText;
    public string NoteTitle => _noteTitle;

    public override void Interact()
    {
        if (!CanInteract) return;

        Debug.Log($"Открыта записка: {_noteTitle}");
    }
}