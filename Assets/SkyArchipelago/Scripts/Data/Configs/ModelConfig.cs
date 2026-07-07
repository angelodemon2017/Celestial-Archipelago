using UnityEngine;

[CreateAssetMenu(menuName = "Models/Base Config")]
public class ModelConfig : BaseDataConfig
{
    public int Uid;
    public EEntityType eEntityType;
    public string ContentName = "New Content";
    public CtxFlag tag;
}