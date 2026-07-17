using System;

[Flags]
public enum CtxFlag : uint
{
    None = 0,
    Damaging = 1 << 0,
    Harvesting = 1 << 1,
    Burning = 1 << 2,
    UIHave = 1 << 3,
    Item = 1 << 4,
    HaveRecipe = 1 << 5,
    HaveContainers = 1 << 5,

    //SomeComboFlag = HaveRecipe | Fueling, - how correct checking
    All = uint.MaxValue
}