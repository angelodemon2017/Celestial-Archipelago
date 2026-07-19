using UnityEngine;

[CreateAssetMenu(menuName = "Craft/Entities Recipes Catalog Config")]
public class EntitiesRecipesCatalogConfig : BaseCatalogConfig<EntityRecipeConfig, int>
{
    protected override void OnValidate()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].UidKeyOfCatalog = i;
        }
    }
}