using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Catalog Entity Configs")]
public class CatalogEntityConfig : ScriptableObject
{
    public List<IslandContentItem> AllContents;

    public EntityViewMB entityViewMB;

    public IslandContentItem GetItemByType(EEntityType eEntityType)
    {
        return AllContents.FirstOrDefault(c => c.eEntityType == eEntityType);
    }
}