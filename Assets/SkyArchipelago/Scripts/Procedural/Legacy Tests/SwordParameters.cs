using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Sword Parameters")]
public class SwordParameters : ScriptableObject
{
    [Header("Handle")]
    public float handleLength = 1.2f;
    public float handleRadius = 0.08f;
    public int handleSegments = 12;

    [Header("Guard")]
    public float guardWidth = 0.6f;
    public float guardThickness = 0.05f;
    public float guardHeight = 0.15f;

    [Header("Blade")]
    public float bladeLength = 2.5f;
    public float bladeWidth = 0.25f;
    public float bladeThickness = 0.04f;
    public float bladeSharpness = 0.92f;
    public int bladeSegments = 24;
    [Header("Blade Shape")]
    public float bladeBaseLength = 1.6f;
    public float bladeTaperScale = 0.35f;
    public float bladeTipLength = 0.9f;

    [Header("Materials")]
    public Material handleMaterial;
    public Material guardMaterial;
    public Material bladeMaterial;
}