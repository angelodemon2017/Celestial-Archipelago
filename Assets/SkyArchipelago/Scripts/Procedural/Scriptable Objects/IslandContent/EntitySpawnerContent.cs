using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Procedural/Island Content/Entity Spawner")]
public class EntitySpawnerContent : IslandContentItem
{
    public DetailOfIslandMB spawnItemPrefab;

    [Header("Cutting Sphere")]
    [Tooltip("Радиус сферы вырезания")]
    public float cutRadius = 6f;

    public override Vector3 GetSizeBound()
    {
        return Vector3.one * (cutRadius * 2.2f);
    }

    public override void Process(MarchingCubesGeneratorController controller, DiContainer diContainer = null)
    {
        if (spawnItemPrefab == null) return;

        var prebInst = Instantiate(spawnItemPrefab, positionOffset + controller.transform.position, Quaternion.Euler(rotationOffset));
        prebInst.transform.SetParent(controller.RootModules);
        if (diContainer != null)
        {
            diContainer.Inject(prebInst);
        }
    }

    public override bool TryGetDensityInfluence(Vector3 pos, Vector3Int gridSize, int seed, out float densityValue)
    {
        Vector3Int center = gridSize / 2 + positionOffset;
        Vector3 localPos = pos - center;
        float distance = localPos.magnitude;

        if (distance > cutRadius + smoothK * 2f)
        {
            densityValue = 0f;
            return false;
        }

        float sphereSDF = distance - cutRadius;
        densityValue = -sphereSDF;
            //sphereSDF;
        return true;
    }
}