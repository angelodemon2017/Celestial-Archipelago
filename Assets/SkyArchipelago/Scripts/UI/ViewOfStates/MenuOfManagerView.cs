using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuOfManagerView : UIWindowBase
{
    [SerializeField] private List<Button> _tabButtons;
    [SerializeField] private Button _buttonTryBuild;
    [SerializeField] private Transform _inventoryParentTransform;

    [Inject] private BuildingModel _buildingModel;
    [Inject] private UIMBFactory<ContainerModel, ItemsListViewMB> _factoryContainer;
    [Inject] private UIMBFactory<int, MenuOfSelectBuild> _factorySelectBuild;
    [Inject] private FPSCommonModel _fPSCommonModel;

    private int _lastIdTab = 0;
    private int _tabCounts;
    private MenuOfSelectBuild _menuOfSelectBuild;
    private ItemsListViewMB _inventoryListView;

    private void Awake()
    {
        _tabCounts = _tabButtons.Count;
        _tabButtons[0].onClick.AddListener(() => OnClickButton(0));
        _tabButtons[1].onClick.AddListener(() => OnClickButton(1));
        _tabButtons[2].onClick.AddListener(() => OnClickButton(2));
//        for (int i = 0; i < _tabCounts; i++)
//            _tabButtons[i].onClick.AddListener(() => OnClickButton(i));
        _buttonTryBuild.onClick.AddListener(SelectBuildStruct);
    }

    public override void Show()
    {
        base.Show();
        OnClickButton(_lastIdTab);
    }

    public override void Hide()
    {
        base.Hide();
        CloseAll();
    }

    private void OnClickButton(int idTab)
    {
        _lastIdTab = idTab;
        CloseAll();
        switch (idTab)
        {
            case 0:
                OpenInventory();
                break;
            case 1:
                OpenBuildsList();
                break;
            case 2:
                break;
            default:
                break;
        }
        _tabButtons[idTab].interactable = false;
    }

    private void CloseAll()
    {
        for (int i = 0; i < _tabCounts; i++)
            _tabButtons[i].interactable = true;
        if (_inventoryListView)
        {
            _factoryContainer.Despawn(_inventoryListView);
            _inventoryListView = null;
        }
        if (_menuOfSelectBuild)
        {
            _factorySelectBuild.Despawn(_menuOfSelectBuild);
            _menuOfSelectBuild = null;
        }
    }

    private void OpenInventory()
    {
        _inventoryListView = _factoryContainer.Create(_fPSCommonModel.ContainerModel, _inventoryParentTransform);
    }

    private void OpenBuildsList()
    {
        _menuOfSelectBuild = _factorySelectBuild.Create(0, _inventoryParentTransform);
    }

    private void SelectBuildStruct()
    {
        _buildingModel.IdEntity = 1;
        _buildingModel.SelectedStruct?.Invoke();
    }

    private void OnDestroy()
    {
        _buttonTryBuild.onClick.RemoveAllListeners();
    }
}