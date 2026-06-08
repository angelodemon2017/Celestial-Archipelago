using UnityEngine;

[System.Serializable]
public class BladeModelShape : BaseModelShape
{
    public float bladeLength = 2.5f;
    public float bladeWidth = 0.25f;
    public float bladeThickness = 0.04f;
    public float bladeSharpness = 0.92f;
    [Header("Blade Shape")]
    public float bladeBaseLength = 1.6f;
    public float bladeTaperScale = 0.35f;
    public float bladeTipLength = 0.9f;
}