using UnityEngine;
using TMPro;
using Zenject;

public class GameplayControllerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;

    private SignalBus _signalBus;
    private DayNightModel _dayNightModel;

    [Inject]
    private void Init(
        SignalBus signalBus,
        DayNightModel dayNightModel)
    {
        _signalBus = signalBus;
        _dayNightModel = dayNightModel;

        Subs();
    }

    private void Subs()
    {
        _signalBus.Subscribe<TimeUpdateSignal>(OnTimeSecond);
    }

    private void OnTimeSecond(TimeUpdateSignal signal)
    {
        _timeText.text = $"D:{_dayNightModel.CurrentDays}. H:{_dayNightModel.CurrentHours}:{_dayNightModel.CurrentMinutes}";
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<TimeUpdateSignal>(OnTimeSecond);
    }
}