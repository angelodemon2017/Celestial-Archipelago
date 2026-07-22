using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class GameplayControllerView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _hintInteractText;
    [SerializeField] private Transform _keyHintsParent;

    private SignalBus _signalBus;
    private DayNightModel _dayNightModel;
    private HinterService _hinterService;
    private UIMBFactory<List<string>, ListOfKeyHintsMB> _listKeysFactory;

    [SerializeField] private ListOfKeyHintsMB _locTemp;

    [Inject]
    private void Init(
        SignalBus signalBus,
        DayNightModel dayNightModel,
        HinterService hinterService,
        UIMBFactory<List<string>, ListOfKeyHintsMB> listKeysFactory)
    {
        _signalBus = signalBus;
        _dayNightModel = dayNightModel;
        _hinterService = hinterService;
        _listKeysFactory = listKeysFactory;

        _hintInteractText.text = string.Empty;

        UpdateHint();
    }

    public override void Show()
    {
        base.Show();
        UpdateHint();
        Subs();
    }

    private void Subs()
    {
        _hinterService.UpdateListHints += UpdateHint;
        _signalBus.Subscribe<TimeUpdateSignal>(OnTimeSecond);
    }

    private void OnTimeSecond(TimeUpdateSignal signal)
    {
        _timeText.text = $"D:{_dayNightModel.CurrentDays.ToTxt()}. H:{_dayNightModel.CurrentHours.ToTxt()}:{_dayNightModel.CurrentMinutes.ToTxt()}";
    }

    private void UpdateHint()
    {
        if (!_locTemp)
            _locTemp = _listKeysFactory.Create(_hinterService.GetHints, _keyHintsParent);
        else
            _locTemp.UpdateList(_hinterService.GetHints);
    }

    public override void Hide()
    {
        base.Hide();
        _hinterService.UpdateListHints -= UpdateHint;
        _signalBus.Unsubscribe<TimeUpdateSignal>(OnTimeSecond);
    }
}