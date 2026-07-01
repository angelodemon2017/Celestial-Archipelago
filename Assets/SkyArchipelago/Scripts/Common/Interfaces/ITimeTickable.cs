public interface ITimeTickable
{
    void OnGameTick(float gameDeltaTime);    // вызывается каждый Update
    // Опционально:
    // void OnFixedGameTick(float fixedDelta);
    // void OnDayChanged();
}