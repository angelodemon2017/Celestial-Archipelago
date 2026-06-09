using System;
using Zenject;

public class DayNightService : IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly DayNightSO _dayNightSO;

    private readonly DayNightModel _dayNightModel;

    public DayNightService(
        SignalBus signalBus,
        DayNightSO dayNightSO,
        DayNightModel dayNightModel)
    {
        _signalBus = signalBus;
        _dayNightSO = dayNightSO;
        _dayNightModel = dayNightModel;

        InitSubs();
    }

    private void InitSubs()
    {
        _signalBus.Subscribe<TimeSecondSignal>(OnTimeSecond);
    }

    private void OnTimeSecond(TimeSecondSignal signal)
    {
        _dayNightModel.CurrentMinutes += 1;
        if (_dayNightModel.CurrentMinutes > _dayNightSO.MinutesInHour)
        {
            _dayNightModel.CurrentMinutes -= _dayNightSO.MinutesInHour;
            _dayNightModel.CurrentHours += 1;
            if (_dayNightModel.CurrentHours > _dayNightSO.HoursInDay)
            {
                _dayNightModel.CurrentHours -= _dayNightSO.HoursInDay;
                _dayNightModel.CurrentDays++;
                if (_dayNightSO.UpdateDays)
                    _signalBus.Fire(new TimeUpdateSignal());
            }
            if (_dayNightSO.UpdateHours)
                _signalBus.Fire(new TimeUpdateSignal());
        }
        if(_dayNightSO.UpdateMinutes)
            _signalBus.Fire(new TimeUpdateSignal());
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<TimeSecondSignal>(OnTimeSecond);
    }
}