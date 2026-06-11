using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Marching Cubes/Figures/Cone Shape Config")]
public class ConeShapeConfigSO : BaseShapeConfigSO
{
    public ConeShape coneShape;

    public override ShapeDefinition GetShape => coneShape;
}