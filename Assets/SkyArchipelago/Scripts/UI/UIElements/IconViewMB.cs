using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class IconViewMB : MonoBehaviour, IPoolable<ItemModel>,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconBackground;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _textSlotId;
    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private TextMeshProUGUI _textDesc;
    [SerializeField] private TextMeshProUGUI _textCounter;
    [SerializeField] private Button _testButton;

    public int SlotId = 0;
    private ItemModel _itemModel;
    private bool _isOverMouse = false;

    public Action<int> BeginerDragAction;
    public Action<int> EndDragAction;
    public Action<int> OnRightClick;
    public Action<int> OnTestButtonSlotIdAction;

    public void Init(ItemModel itemModel)
    {
        _itemModel = itemModel;
        SlotId = _itemModel.SlotId;
        _itemModel.Changed += UpdateView;
        UpdateView();
    }

    private void UpdateView()
    {
        ApplyRarityItem(_itemModel.RarityItem);
        _testButton.gameObject.SetActive(_itemModel.TypeItem != EItemType.None);
        _textSlotId.text = _itemModel.SlotId.ToTxt();
        _textTitle.text = _itemModel.FullItemName;
        _textDesc.text = _itemModel.Description;
        _textCounter.text = _itemModel.Count == 0 ?
            string.Empty :
            (_itemModel.MaxStack > 1 ?
            _itemModel.Count.ToTxt() : string.Empty);
    }

    private void ApplyRarityItem(ERarityItem rarityItem)
    {
        switch (rarityItem)
        {
            case ERarityItem.Trash:
                _iconBackground.color = Color.gray;
                break;
            case ERarityItem.Normal:
                _iconBackground.color = Color.white;
                break;
            case ERarityItem.Uncommon:
                _iconBackground.color = Color.limeGreen;
                break;
            default:
                _iconBackground.color = Color.white;
                break;
        }
    }

    private void OnEnable()
    {
        _testButton.onClick.AddListener(OnClickButton);
    }

    private void Update()
    {
        if (_isOverMouse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginerDragAction?.Invoke(SlotId);
            }
            if (Input.GetMouseButtonUp(0))
            {
                EndDragAction?.Invoke(SlotId);
            }
            if (Input.GetMouseButtonUp(1))
            {
                OnRightClick?.Invoke(SlotId);
            }
        }
    }

    private void OnClickButton()
    {
        OnTestButtonSlotIdAction?.Invoke(_itemModel.SlotId);
    }

    private void OnDisable()
    {
        SetMouseWaiting(false);
        _testButton.onClick.RemoveAllListeners();
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
        SetMouseWaiting(false);
        gameObject.SetActive(false);
    }

    public void Clean()
    {
        _iconBackground.color = Color.white;
        _iconImage.sprite = null;
        _textTitle.color = Color.black;
        _textTitle.text = string.Empty;
        _textDesc.text = string.Empty;
        _textCounter.text = string.Empty;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetMouseWaiting(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetMouseWaiting(true);
    }

    public void SetMouseWaiting(bool isOn)
    {
        _isOverMouse = isOn;
        _iconImage.enabled = !_isOverMouse;
    }
}