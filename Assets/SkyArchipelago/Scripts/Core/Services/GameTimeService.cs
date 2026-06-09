using Zenject;

public class GameTimeService : ITickable
{
    private readonly SignalBus _signalBus;
    private readonly SystemSO _systemSO;

    private float _time;

    public GameTimeService(
        SignalBus signalBus,
        SystemSO systemSO)
    {
        _signalBus = signalBus;
        _systemSO = systemSO;
    }

    public void Tick()
    {
        _time += UnityEngine.Time.deltaTime * _systemSO.timeScale;

        while (_time > 1f)
        {
            _signalBus.Fire(new TimeSecondSignal());
            _time -= 1f;
        }
    }
}