using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TextLabelOfVerticalbarMB : MonoBehaviour, IPoolable<string>
{
    [SerializeField] private TextMeshProUGUI _someText;
    [SerializeField] private Image _sideIcon;

    public void OnSpawned(string p1)
    {
//        _sideIcon.color = Random.ColorHSV();
        _someText.text = p1;
    }

    public void OnDespawned()
    {
        _someText.text = string.Empty;
    }
}