using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuControllerView : UIWindowBase
{
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _settingButton;
    [SerializeField] private Button _extButton;

    private SignalBus _signalBus;

    [Inject]
    private void Init(
        SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Awake()
    {
        _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        _settingButton.onClick.AddListener(OnSettingButtonClicked);
        _extButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnNewGameButtonClicked()
    {
        _signalBus.Fire(new LoadSceneSignal(Dicts.Scenes.Island));
    }

    private void OnSettingButtonClicked()
    {

    }

    private void OnExitButtonClicked()
    {

    }

    private void OnDestroy()
    {
        _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
        _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        _extButton.onClick.RemoveListener(OnExitButtonClicked);
    }
}