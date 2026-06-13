using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    public PlayerController PlayerControllerPrefab;

    public float mouseSensitivity = 100f;

    public LayerMask FPSLayer;
    public LayerMask BuildingPlacement;
}