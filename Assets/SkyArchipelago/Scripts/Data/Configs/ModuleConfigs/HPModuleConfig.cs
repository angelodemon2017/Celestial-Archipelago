using UnityEngine;

[CreateAssetMenu(menuName = "Modules/HP Module Config")]
public class HPModuleConfig : ModuleConfig
{
    public override CtxFlag KeyFlag => CtxFlag.Damaging;

    public int BaseHp;
}