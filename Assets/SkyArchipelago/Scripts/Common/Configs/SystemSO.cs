using UnityEngine;

[CreateAssetMenu(menuName = "Core/SystemSO")]
public class SystemSO : ScriptableObject
{
    public float timeScale = 1f;
    public float gameTick = 0.1f;
    public float globalAutoCraftAction = 0.1f;
    public float globalBurnPeriod = 0.1f;
    public float globalSpawnTimer = 2f;

    public ModelConfig ConfigDropItem;
    public LayerMask SpawnRaycastMask;
}