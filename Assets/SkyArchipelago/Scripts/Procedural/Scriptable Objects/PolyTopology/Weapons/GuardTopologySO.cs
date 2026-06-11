using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Guard Topology")]
public class GuardTopologySO : BaseMeshTopologySO
{
    public GuardModelShape guardModelShape;

    public override ProceduralMeshGenerator MeshGenerator => new CrossGuardMeshGenerator(this);

    public override BaseModelShape DefShape => guardModelShape;
}