using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Catalog Entity Configs")]
public class CatalogEntityConfig : ScriptableObject
{
    public List<ModelConfig> AllModels;

    public EntityViewMB entityViewMB;

    public ModelConfig GetModelConfigByType(EEntityType eEntityType)
    {
        return AllModels.FirstOrDefault(c => c.eEntityType == eEntityType);
    }
}