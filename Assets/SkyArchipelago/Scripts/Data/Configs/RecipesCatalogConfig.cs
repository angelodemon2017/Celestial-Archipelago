using UnityEngine;

[CreateAssetMenu(menuName = "Craft/Recipes Catalog Config")]
public class RecipesCatalogConfig : BaseCatalogConfig<RecipeConfig, int>
{
    protected override void OnValidate()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].KeyOfCatalog = i;
        }
    }
}