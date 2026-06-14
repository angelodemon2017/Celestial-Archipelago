using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuOfManagerView : UIWindowBase
{
    [SerializeField] private Button _button1;
    [SerializeField] private Button _button2;
    [SerializeField] private Button _button3;
    [SerializeField] private Button _buttonTryBuild;

    [Inject] private BuildingModel _buildingModel;

    private void Awake()
    {
        _button1.onClick.AddListener(() => OnClickButton(1));
        _button2.onClick.AddListener(() => OnClickButton(2));
        _button3.onClick.AddListener(() => OnClickButton(3));
        _buttonTryBuild.onClick.AddListener(SelectBuildStruct);
    }

    private void OnClickButton(int testCounter)
    {
        Debug.Log($"OnClickButton {testCounter}");
    }

    private void SelectBuildStruct()
    {
        _buildingModel.IdStruct = 1;
        _buildingModel.SelectedStruct?.Invoke();
    }

    private void OnDestroy()
    {
        _button1.onClick.RemoveAllListeners();
        _button2.onClick.RemoveAllListeners();
        _button3.onClick.RemoveAllListeners();
        _buttonTryBuild.onClick.RemoveAllListeners();
    }
}