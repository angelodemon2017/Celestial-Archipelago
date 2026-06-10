public interface IGameTimeService
{
    float TimeScale { get; }
    void Register(ITimeTickable tickable);
    void Unregister(ITimeTickable tickable);
}