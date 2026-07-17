using TMPro;
using UnityEngine;
using Zenject;

public class TextLabelOfVerticalbarMB : MonoBehaviour, IPoolable<string>
{
    [SerializeField] private TextMeshProUGUI _someText;

    public void OnSpawned(string p1)
    {
        _someText.text = p1;
    }

    public void OnDespawned()
    {
        _someText.text = string.Empty;
    }
}