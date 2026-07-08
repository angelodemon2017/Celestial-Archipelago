using UnityEngine;

[CreateAssetMenu(menuName = "Containers/ContainerData")]
public class ContainerConfig : BaseDataConfig
{
    public string KeyName;
    public EContainerType containerType;
    public byte BaseSlots;
    public int CustomStackSize = 0;
    public CtxFlag AvailableTagItems = CtxFlag.All;
}