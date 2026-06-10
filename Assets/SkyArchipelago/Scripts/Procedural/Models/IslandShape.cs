[System.Serializable]
public class IslandShape : BaseModelShape
{
    public float seedOffset = 0f;
    public float plateauFlatness = 0.85f;   // насколько плоский верх
    public float noiseScale = 1.8f;
    public float noiseStrength = 0.6f;
}