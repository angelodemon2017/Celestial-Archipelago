using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Containers Config")]
public class ContainersModuleConfig : ModuleConfig
{
    public override CtxFlag KeyFlag => CtxFlag.HaveContainers;
}