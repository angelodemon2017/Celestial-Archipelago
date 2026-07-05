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
        UpdateView();
    }

    private void UpdateView()
    {
        CleanIcons();
        foreach (var item in _containerModel.itemModels)
        {
            var view = _iconViewFactory.Create(item, _contentParent);
            _iconViewMBs.Add(view);
        }
    }

    public void OnDespawned()
    {
        _containerModel.Changed -= UpdateView;
        _containerModel = null;
        CleanIcons();
        gameObject.SetActive(false);
    }

    private void CleanIcons()
    {
        _iconViewMBs.ForEach(iv => _iconViewFactory.Despawn(iv));
        _iconViewMBs.Clear();
    }
}