using UnityEngine;

[CreateAssetMenu(menuName = "Modules/UIModule Config")]
public class UIModuleConfig : ModuleConfig
{
    public override CtxFlag KeyFlag => CtxFlag.UIHave;

    public BaseViewOfModelEntity View;
}