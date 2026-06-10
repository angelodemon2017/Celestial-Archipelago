using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameTimeService : IGameTimeService, ITickable
{
    private readonly SystemSO _systemSO;
    private readonly List<ITimeTickable> tickables = new ();

    public float TimeScale => _systemSO.timeScale;//and potencial other time modifiers

    public GameTimeService(
        SystemSO systemSO) 
    {
        _systemSO = systemSO;
    }

    public void Tick()
    {
        float realDelta = Time.unscaledDeltaTime;
        float gameDelta = realDelta * TimeScale;

        for (int i = tickables.Count - 1; i >= 0; i--)
        {
            tickables[i].OnGameTick(gameDelta);
        }
    }

    public void Register(ITimeTickable t) => tickables.Add(t);
    public void Unregister(ITimeTickable t) => tickables.Remove(t);
}