using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Marching Cubes/Figures/Sphere Shape Config")]
public class SphereShapeConfigSO : BaseShapeConfigSO
{
    public SphereShape sphereShape;

    public override ShapeDefinition GetShape => sphereShape;
}