using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modules/Harvest Config")]
public class HarvestConfig : ModuleConfig
{
    public override CtxFlag KeyFlag => CtxFlag.Harvesting;

    public List<ItemAmount> _outputs;
    public CtxFlag _availableItem;
}