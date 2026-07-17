using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ItemsListViewMB : MonoBehaviour, IPoolable<ContainerModel>
{
    [SerializeField] private TextMeshProUGUI _textTitleContainer;
    [SerializeField] private Transform _contentParent;

    [Inject] private UIDragAndDropItemsService _dragAndDropItemsService;
    [Inject] private UIMBFactory<ItemModel, IconViewMB> _iconViewFactory;

    private int _lastFocusItem = -1;
    public ContainerModel _containerModel;//need model of view?
    private List<IconViewMB> _iconViewMBs = new();

    public void OnSpawned(ContainerModel containerModel)
    {
        _containerModel = containerModel;
        _containerModel.Changed += UpdateView;
        _containerModel.ChangedSlotId += UpdateSlot;
        UpdateView();
    }

    private void UpdateView()
    {
        _textTitleContainer.text = _containerModel.TitleContainer;
        CleanIcons();

        var countIcons = _containerModel.itemModels.Count;
        for (int i = 0; i < countIcons; i++)
        {
            var view = _iconViewFactory.Create(_containerModel.GetItemBySlot(i), _contentParent);
            Sub(view);
            _iconViewMBs.Add(view);
        }
    }

    private void UpdateSlot(int idSlot)
    {
        var oldView = _iconViewMBs[idSlot];
        UnSub(oldView);
        _iconViewFactory.Despawn(oldView);
        var model = _containerModel.GetItemBySlot(idSlot);
        var view = _iconViewFactory.Create(model, _contentParent);
        Sub(view);
        view.SetMouseWaiting(_lastFocusItem == idSlot);
        _iconViewMBs[idSlot] = view;
        view.transform.SetSiblingIndex(idSlot);
    }

    private void Sub(IconViewMB newSub)
    {
        newSub.OnTestButtonSlotIdAction += ClickedOnButtonOfSlot;
        newSub.BeginerDragAction += OnBeginDragSlotId;
        newSub.EndDragAction += OnEndDropSlotId;
        newSub.OnRightClick += OnRightClick;
    }

    private void ClickedOnButtonOfSlot(int id)
    {

    }

    private void OnBeginDragSlotId(int id)
    {

    }

    private void OnEndDropSlotId(int id)
    {
        _lastFocusItem = id;
        _dragAndDropItemsService.ClickLMBBySlot(id, _containerModel);
    }

    private void OnRightClick(int id)
    {
        _lastFocusItem = id;
        _dragAndDropItemsService.ClickRMBBySlot(id, _containerModel);
    }

    private void UnSub(IconViewMB deletingIcon)
    {
        deletingIcon.OnTestButtonSlotIdAction -= ClickedOnButtonOfSlot;
        deletingIcon.BeginerDragAction -= OnBeginDragSlotId;
        deletingIcon.EndDragAction -= OnEndDropSlotId;
        deletingIcon.OnRightClick -= OnRightClick;
    }

    public void OnDespawned()
    {
        _lastFocusItem = -1;
        _dragAndDropItemsService.DragReturnToSelfContainer();
        if (_containerModel != null)
        {
            _containerModel.Changed -= UpdateView;
            _containerModel.ChangedSlotId -= UpdateSlot;
            _containerModel = null;
        }
        CleanIcons();
        gameObject.SetActive(false);
    }

    private void CleanIcons()
    {
        var countIcons = _iconViewMBs.Count;
        for (int i = 0; i < countIcons; i++)
        {
            var item = _iconViewMBs[i];
            UnSub(item);
            _iconViewFactory.Despawn(item);
        }
        _iconViewMBs.Clear();
    }
}