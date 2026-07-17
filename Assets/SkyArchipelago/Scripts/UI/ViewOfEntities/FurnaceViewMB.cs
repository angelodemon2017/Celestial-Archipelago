using TMPro;
using UnityEngine;
using Zenject;

public class FurnaceViewMB : BaseViewOfModelEntity<FurnaceModel>
{
    [SerializeField] private TextMeshProUGUI _textTitleContainer;
    [SerializeField] private TextMeshProUGUI _textProgress;
    [SerializeField] private TextMeshProUGUI _textFuelStorage;
    [SerializeField] private Transform _inputContainerParent;
    [SerializeField] private Transform _outputContainerParent;
    [SerializeField] private Transform _fuelContainerParent;
    [SerializeField] private Transform _playerContainerParent;

    [Inject] private BurningFuelsRepository _burningFuelsRepository;
    [Inject] private CraftProcessRepository _craftProcessRepository;

    private ItemsListViewMB _inputContainerView;
    private ItemsListViewMB _outputContainerView;
    private ItemsListViewMB _fuelContainerView;
    private ItemsListViewMB _playerInventoryView;

    protected override void UpdateViewWithSettedModel()
    {
        base.UpdateViewWithSettedModel();
        _textTitleContainer.text = PlayerContainer.TitleContainer;
        if (!_inputContainerView)
            _inputContainerView = GetILVMB(_model, EContainerType.SourceInput, _inputContainerParent);
        if (!_outputContainerView)
            _outputContainerView = GetILVMB(_model, EContainerType.ProductionOutput, _outputContainerParent);
        if (!_fuelContainerView)
            _fuelContainerView = GetILVMB(_model, EContainerType.BurningFuel, _fuelContainerParent);
        if (!_playerInventoryView)
            _playerInventoryView = GetILVMB(PLM, EContainerType.BasePlayer, _playerContainerParent);
        UpdateUAProgress();
        UpdateFuelStorage();
    }

    private void UpdateUAProgress()
    {
        if (_craftProcessRepository.TryGetCraftById(_model.CraftIdProcess, out var craft))
        {
            var cur = craft.Process;
            _textProgress.text = $"{cur.ToTxt()}/{craft.ConfigModel.ActionUnitsRequired.ToTxt()}";
        }
        else
        {
            _textProgress.text = "---";
        }
    }

    private void UpdateFuelStorage()
    {
        _textFuelStorage.text =
            _burningFuelsRepository.TryGetBurnById(_model.BurnIdProcess, out var burn) ?
            burn.Storage.ToTxt() :
            "---";
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        if (_inputContainerView)
        {
            _factoryContainer.Despawn(_inputContainerView);
            _inputContainerView = null;
        }
        if (_outputContainerView)
        {
            _factoryContainer.Despawn(_outputContainerView);
            _outputContainerView = null;
        }
        if (_fuelContainerView)
        {
            _factoryContainer.Despawn(_fuelContainerView);
            _fuelContainerView = null;
        }
        if (_playerInventoryView)
        {
            _factoryContainer.Despawn(_playerInventoryView);
            _playerInventoryView = null;
        }
        gameObject.SetActive(false);
    }
}