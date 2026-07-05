using UnityEngine;

[CreateAssetMenu(menuName = "Containers/ContainerData")]
public class ContainerConfig : BaseDataConfig
{
    public EContainerType containerType;
    public int BaseSlots;//example parametrs
    public int CustomStackSize = 0;
    public CtxFlag Availableitems = CtxFlag.All;
}