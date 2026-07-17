using TMPro;
using UnityEngine;

public class SimpleChestViewMB : BaseViewOfModelEntity<WoodChestModel>
{
    [SerializeField] private TextMeshProUGUI _textTitleChestContainer;
    [SerializeField] private Transform _chestContainerParent;
    [SerializeField] private TextMeshProUGUI _textTitlePlayerContainer;
    [SerializeField] private Transform _playerContainerParent;

    private ItemsListViewMB _chestContainerView;
    private ItemsListViewMB _playerInventoryView;

    protected override void UpdateViewWithSettedModel()
    {
        base.UpdateViewWithSettedModel();
        _textTitleChestContainer.text = _model.ModelName;
        _textTitlePlayerContainer.text = PlayerContainer.TitleContainer;

        if (!_chestContainerView)
            _chestContainerView = GetILVMB(_model, _model.MainContainer, _chestContainerParent);
        if (!_playerInventoryView)
            _playerInventoryView = GetILVMB(PLM, EContainerType.BasePlayer, _playerContainerParent);
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        if (_chestContainerView)
        {
            _factoryContainer.Despawn(_chestContainerView);
            _chestContainerView = null;
        }
        if (_playerInventoryView)
        {
            _factoryContainer.Despawn(_playerInventoryView);
            _playerInventoryView = null;
        }
        gameObject.SetActive(false);
    }
}