using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRecipeConfig : BaseDataConfig, BaseCatalogElementConfig<int>
{
    public int Uid;
    public List<ItemAmount> _inputs;

    public abstract string GetTitle { get; }
    public int UidKeyOfCatalog
    {
        get => Uid;
        set => Uid = value;
    }
}

[System.Serializable]
public struct ItemAmount
{
    public ItemConfig Config;
    [Min(1)]
    public int Amount;
    public float Chance;
}