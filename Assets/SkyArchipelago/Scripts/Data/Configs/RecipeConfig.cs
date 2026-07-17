using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Craft/Recipe Config")]
public class RecipeConfig : BaseDataConfig, BaseCatalogElementConfig<int>
{
    public int Uid;
    public List<ItemAmount> _inputs;
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

    public int KeyOfCatalog
    {
        get => Uid;
        set => Uid = value;
    }
}

[System.Serializable]
public struct ItemAmount
{
    public ItemConfig Config;
    public int Amount;
    public float Chance;
}