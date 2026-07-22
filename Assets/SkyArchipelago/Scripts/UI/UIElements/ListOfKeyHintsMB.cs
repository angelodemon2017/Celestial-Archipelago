using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ListOfKeyHintsMB : MonoBehaviour, IPoolable<List<string>>
{
    [SerializeField] private Transform _parentHints;

    [Inject] private UIMBFactory<string, TextLabelOfVerticalbarMB> _labelFactory;

    private List<TextLabelOfVerticalbarMB> _textLabelOfs = new();

    public void OnSpawned(List<string> hints)
    {
        ShowList(hints);
    }

    public void UpdateList(List<string> hints)
    {
        CleanCurrentList();
        ShowList(hints);
    }

    private void ShowList(List<string> hints)
    {
        int count = hints.Count;
        for (int i = 0; i < count; i++)
            _textLabelOfs.Add(_labelFactory.Create(hints[i], _parentHints));
    }

    private void CleanCurrentList()
    {
        int count = _textLabelOfs.Count;
        for (int i = 0; i < count; i++)
            _labelFactory.Despawn(_textLabelOfs[i]);
        _textLabelOfs.Clear();
    }

    public void OnDespawned()
    {
        CleanCurrentList();
    }
}