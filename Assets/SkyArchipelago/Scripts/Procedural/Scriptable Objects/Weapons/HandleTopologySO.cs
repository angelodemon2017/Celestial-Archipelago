using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Handle Topology")]
public class HandleTopologySO : BaseMeshTopologySO
{
    public HandleModelShape handleModelShape;

    public int handleSegments = 12;
    public int rings = 18;
    public int segments = 24;

    public override BaseModelShape DefShape => handleModelShape;
}