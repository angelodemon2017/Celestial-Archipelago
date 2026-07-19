using UnityEngine;

[CreateAssetMenu(menuName = "Craft/Entity Recipe Config")]
public class EntityRecipeConfig : BaseRecipeConfig//, BaseCatalogElementConfig<int>
{
    public ModelConfig EntityConfig;
    [Min(1)]
    public int ActionUnitsRequired = 1;
    public bool IsDeleteInputsOnPlacement;
    public EEntityCategory eEntityCategory;

    public override string GetTitle => EntityConfig.ContentName;
}