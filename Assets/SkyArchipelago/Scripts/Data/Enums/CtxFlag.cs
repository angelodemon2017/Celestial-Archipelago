using System;

[Flags]
public enum CtxFlag : uint
{
    None = 0,
    Damaging = 1 << 0,
    Harvesting = 1 << 1,
    Fueling = 1 << 2,
    UIHave = 1 << 3,
    Item = 1 << 4,
    All = uint.MaxValue
}