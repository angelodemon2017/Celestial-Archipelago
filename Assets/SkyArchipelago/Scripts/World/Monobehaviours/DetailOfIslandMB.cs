using UnityEngine;
using Zenject;

public class DetailOfIslandMB : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    private SignalBus _signalBus;
    private PointsRepository _pointsRepository;

    [Inject]
    private void Init(
        SignalBus signalBus,
        PointsRepository pointsRepository)
    {
        _signalBus = signalBus;
        _pointsRepository = pointsRepository;

        _pointsRepository.RegisterPoint(_spawnPoint.position);
        _signalBus.Fire(new LaunchSpawnPointSignal(_spawnPoint.position));
    }
}