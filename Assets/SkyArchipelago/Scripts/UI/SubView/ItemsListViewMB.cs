using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class ItemsListViewMB : MonoBehaviour, IPoolable<ContainerModel>
{
    [SerializeField] private TextMeshProUGUI _textTitleContainer;
    [SerializeField] private Transform _contentParent;

    [Inject] private UIMBFactory<ItemModel, IconViewMB> _iconViewFactory;

    private ContainerModel _containerModel;
    private List<IconViewMB> _iconViewMBs = new();

    public void OnSpawned(ContainerModel contModel)
    {
        Init(contModel);
    }

    public void Init(ContainerModel containerModel)
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
        foreach (var item in _containerModel.itemModels)
        {
            var view = _iconViewFactory.Create(item, _contentParent);
            view.OnTestButtonSlotIdAction += ClickedOnSlot;
            _iconViewMBs.Add(view);
        }
    }

    private void UpdateSlot(byte id)
    {
        var oldView = _iconViewMBs[id];
        oldView.OnTestButtonSlotIdAction -= ClickedOnSlot;
        _iconViewFactory.Despawn(oldView);

        var model = _containerModel.itemModels[id];
        var view = _iconViewFactory.Create(model, _contentParent);
        view.OnTestButtonSlotIdAction += ClickedOnSlot;
        _iconViewMBs[id] = view;
    }

    private void ClickedOnSlot(byte id)
    {
        _containerModel.TestActionBySlot?.Invoke(_containerModel.Id, id);
    }

    public void OnDespawned()
    {
        _containerModel.Changed -= UpdateView;
        _containerModel.ChangedSlotId -= UpdateSlot;
        _containerModel = null;
        CleanIcons();
        gameObject.SetActive(false);
    }

    private void CleanIcons()
    {
        foreach (var item in _iconViewMBs)
        {
            item.OnTestButtonSlotIdAction -= ClickedOnSlot;
            _iconViewFactory.Despawn(item);
        }
        _iconViewMBs.Clear();
    }
}