using UnityEngine;

[CreateAssetMenu(menuName = "Models/Base Config")]
public class ModelConfig : BaseDataConfig
{
    public EEntityType eEntityType;
    public string ContentName = "New Content";
    public CtxFlag tag;
    public GameObject ViewModelPrefab;
}