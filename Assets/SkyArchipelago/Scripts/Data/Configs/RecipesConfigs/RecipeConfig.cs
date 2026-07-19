using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Craft/Item Recipe Config")]
public class RecipeConfig : BaseRecipeConfig, BaseCatalogElementConfig<int>
{
    public List<ItemAmount> _outputs;
    [Min(1)]
    public int ActionUnitsRequired = 1;
    [Min(1)]
    public int AvailableAUByTick = 1;
    public bool IsCalcingInAutoCraftTicks = true;
    /// <summary>
    /// false - delete inputs after done
    /// was problem in TryAutoSelectingCraft() - how calc 
    /// </summary>
    public bool IsDeleteInputsOnStart => false;
    /// <summary>
    /// true - only player create craft process
    /// false - furnacing for example
    /// </summary>
    public bool IsStaticRecipe = true;
    public bool IsReturnResourceAfterStopCraft = true;

    public override string GetTitle => _outputs[0].Config.KeyName;
}