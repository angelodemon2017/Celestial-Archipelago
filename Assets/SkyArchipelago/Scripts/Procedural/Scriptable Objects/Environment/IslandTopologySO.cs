using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Topology/Island Topology")]
public class IslandTopologySO : BaseMeshTopologySO
{
    public IslandShape shape;
    public override BaseModelShape DefShape => shape;
    public override ProceduralMeshGenerator MeshGenerator => new IslandMeshGenerator(this);

    [Header("Island Size")]
    public float plateauRadius = 35f;      // Радиус верхнего плато
    public float plateauHeight = 8f;       // Толщина плато
    public float totalHeight = 45f;        // Общая высота острова (от низа сталактитов до верха)

    [Header("Stalactites")]
    public int stalactiteCount = 12;
    public float maxStalactiteLength = 28f;
    public float stalactiteBaseRadius = 6f;
    public float stalactiteTipRadius = 0.8f;

    [Header("Top Plateau")]
    public float maxHeightVariation = 1.2f; // минимальные перепады наверху
    public int plateauSubdivision = 24;
}