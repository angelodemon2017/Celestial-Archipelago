using System;
using Zenject;

public class DayNightService : ITimeTickable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly DayNightSO _dayNightSO;
    private readonly IGameTimeService _timeService;

    private readonly DayNightModel _dayNightModel;
    private float _realTimer;

    public float GetNormalizedTimeOfDay() =>
        (float)(_dayNightModel.CurrentHours * _dayNightSO.MinutesInHour + _dayNightModel.CurrentMinutes) / (float)(_dayNightSO.HoursInDay * _dayNightSO.MinutesInHour);

    public DayNightService(
        SignalBus signalBus,
        DayNightSO dayNightSO,
        DayNightModel dayNightModel,
        IGameTimeService timeService)
    {
        _signalBus = signalBus;
        _dayNightSO = dayNightSO;
        _dayNightModel = dayNightModel;
        _timeService = timeService;

        _dayNightModel.CurrentHours = _dayNightSO.StartHourByCreatedWorld;
        _timeService.Register(this);
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _realTimer += gameDeltaTime;
        while (_realTimer >= _dayNightSO.RealSecondsInGameMinutes)
        {
            NextGameMinute();
            _realTimer -= _dayNightSO.RealSecondsInGameMinutes;
        }
    }

    private void NextGameMinute()
    {
        _dayNightModel.CurrentMinutes++;
        if (_dayNightModel.CurrentMinutes > _dayNightSO.MinutesInHour)
        {
            NextGameHour();
            _dayNightModel.CurrentMinutes -= _dayNightSO.MinutesInHour;
        }
        else if (_dayNightSO.UpdateMinutes)
            _signalBus.Fire(new TimeUpdateSignal());
    }

    private void NextGameHour()
    {
        _dayNightModel.CurrentHours++;
        if (_dayNightModel.CurrentHours > _dayNightSO.HoursInDay)
        {
            _dayNightModel.CurrentHours -= _dayNightSO.HoursInDay;
            NextGameDay();
        }
        else if (_dayNightSO.UpdateHours)
            _signalBus.Fire(new TimeUpdateSignal());
    }

    private void NextGameDay()
    {
        _dayNightModel.CurrentDays++;
        if (_dayNightSO.UpdateDays)
            _signalBus.Fire(new TimeUpdateSignal());
    }

    public void Dispose()
    {
        _timeService.Unregister(this);
    }
}