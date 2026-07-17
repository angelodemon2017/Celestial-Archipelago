using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/Model Config")]
public class ModelConfig : BaseDataConfig
{
    public int Uid;
    public EEntityType eEntityType;
    public string ContentName = "New Content";
    public List<ModuleConfig> ModuleConfigs;
}