using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Spawner Drops Module Config")]
public class SpawnerDropsModuleConfig : ModuleConfig
{
    [Range(0f,1f)]
    public float BaseChance = 0.1f;
    [Range(0f, 1f)]
    public float IncChanceByFall = 0.02f;
    public int MaxDrops = 1;
    public float Radius = 3f;
    public List<SpawnParametrs> ItemVariants;

    public override CtxFlag KeyFlag => CtxFlag.ItemSpawner;

    [System.Serializable]
    public class SpawnParametrs : IWeighted
    {
        public ItemConfig ItemConfig;
        public int WeightRnd;
        public int Min;
        public int Max;

        public int Weight => WeightRnd;
    }
}