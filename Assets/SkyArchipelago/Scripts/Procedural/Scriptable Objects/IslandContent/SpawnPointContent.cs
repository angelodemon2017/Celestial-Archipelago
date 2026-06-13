using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Procedural/Island Content/Spawn Point")]
public class SpawnPointContent : IslandContentItem
{
    public Transform spawnPointReference; // ссылка на объект в сцене

    public override void Process(MarchingCubesGeneratorController controller, DiContainer diContainer)
    {
        if (spawnPointReference == null) return;

        // Находим самую высокую точку (можно улучшить)
//        float maxY = controller.GetHighestPoint();

        Vector3 pos = controller.transform.position + positionOffset;
  //      pos.y = maxY + 3f;

        spawnPointReference.position = pos;
        spawnPointReference.rotation = Quaternion.Euler(rotationOffset);

        Debug.Log($"Spawn point placed at {pos}");
    }
}