using UnityEngine;

[CreateAssetMenu(fileName = "BuildFPS State Config", menuName = "States/BuildFPS State Config")]
public class BuildFPSStateConfig : ScriptableObject
{
    public BuildMarkerMB buildMarkerPrefab;
    public Material ValidMaterial;
    public Material InValidMaterial;
}