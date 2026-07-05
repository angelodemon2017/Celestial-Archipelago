using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class IconViewMB : MonoBehaviour, IPoolable<ItemModel>
{
    [SerializeField] private Image _iconBackground;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private TextMeshProUGUI _textDesc;
    [SerializeField] private TextMeshProUGUI _textCounter;

    private ItemModel _itemModel;

    public void Clean()
    {
        _iconBackground.color = Color.white;
        _iconImage.sprite = null;
        _textTitle.color = Color.black;
        _textTitle.text = string.Empty;
        _textDesc.text = string.Empty;
        _textCounter.text = string.Empty;
    }

    public void Init(ItemModel itemModel)
    {
        _itemModel = itemModel;
        _itemModel.Changed += UpdateView;
        UpdateView();
    }

    public void UpdateView()
    {
        _textTitle.text = _itemModel.FullItemName;
        _textDesc.text = _itemModel.Description;
        _textCounter.text = _itemModel.Count == 0 ?
            string.Empty :
            $"{_itemModel.Count}";
    }

    public void OnSpawned(ItemModel itemModel)
    {
        _itemModel = itemModel;
        gameObject.SetActive(true);
        Clean();
        Init(itemModel);
    }

    public void OnDespawned()
    {
        if(_itemModel != null)
            _itemModel.Changed -= UpdateView;
        _itemModel = null;
        Clean();
        gameObject.SetActive(false);
    }
}