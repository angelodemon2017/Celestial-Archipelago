using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    public float mouseSensitivity = 100f;

    public LayerMask FPSLayer;
    public LayerMask FPSBuildingLayer;
}