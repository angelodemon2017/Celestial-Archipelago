using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Marching Cubes/Catalog Configs")]
public class CatalogIslandConfigs : ScriptableObject
{
    public IslandViewMB IslandPrefab;

    public List<MarchingCubesConfigSO> Countering;
    public List<MarchingCubesConfigSO> TestIslands;

    public MarchingCubesConfigSO StartIslandConfig;

    private void OnValidate()
    {
        for (int i = 0; i < Countering.Count; i++)
        {
            if (Countering[i] != null)
                Countering[i].IdConfig = i;
        }
    }
}