using UnityEngine;

public abstract class ModuleConfig : ScriptableObject
{
    public abstract CtxFlag KeyFlag { get; }
}