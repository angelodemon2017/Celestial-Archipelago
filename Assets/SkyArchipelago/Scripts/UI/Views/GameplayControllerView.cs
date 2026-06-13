using UnityEngine;
using TMPro;
using Zenject;

public class GameplayControllerView : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _hintInteractText;

    private SignalBus _signalBus;
    private DayNightModel _dayNightModel;
    private HinterService _hinterService;

    [Inject]
    private void Init(
        SignalBus signalBus,
        DayNightModel dayNightModel,
        HinterService hinterService)
    {
        _signalBus = signalBus;
        _dayNightModel = dayNightModel;
        _hinterService = hinterService;

        Subs();

        UpdateHint();
    }

    private void OnEnable()
    {
        UpdateHint();
    }

    private void Subs()
    {
        _hinterService.UpdatedHint += UpdateHint;
        _signalBus.Subscribe<TimeUpdateSignal>(OnTimeSecond);
    }

    private void OnTimeSecond(TimeUpdateSignal signal)
    {
        _timeText.text = $"D:{_dayNightModel.CurrentDays}. H:{_dayNightModel.CurrentHours}:{_dayNightModel.CurrentMinutes}";
    }

    private void UpdateHint()
    {
        _hintInteractText.text =
            _hinterService == null ?
            string.Empty :
            _hinterService.Hint;
    }

    private void OnDestroy()
    {
        _hinterService.UpdatedHint -= UpdateHint;
        _signalBus.Unsubscribe<TimeUpdateSignal>(OnTimeSecond);
    }
}