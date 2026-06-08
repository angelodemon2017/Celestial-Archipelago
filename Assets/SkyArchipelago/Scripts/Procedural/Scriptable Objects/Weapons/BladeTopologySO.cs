using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Blade Topology")]
public class BladeTopologySO : BaseMeshTopologySO
{
    public BladeModelShape bladeModelShape;

    public int bladeSegments = 24;

    public override BaseModelShape DefShape => bladeModelShape;
}