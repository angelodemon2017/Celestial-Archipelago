using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuControllerView : UIWindowBase
{
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _extButton;

    private MainMenuModel _mainMenuModel;

    [Inject]
    private void Init(
        MainMenuModel mainMenuModel)
    {
        _mainMenuModel = mainMenuModel;
    }

    private void Awake()
    {
        _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _extButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        _mainMenuModel.OnNewGameClick?.Invoke();
    }

    private void OnSettingButtonClicked()
    {
        _mainMenuModel.OnSettingClick?.Invoke();
    }

    private void OnExitButtonClicked()
    {
        _mainMenuModel.OnExitClick?.Invoke();
    }

    private void OnDestroy()
    {
        _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        _extButton.onClick.RemoveListener(OnExitButtonClicked);
    }
}