using UnityEngine;

[CreateAssetMenu(menuName = "Containers/ContainerData")]
public class ContainerConfig : BaseDataConfig, BaseCatalogElementConfig<EContainerType>
{
    public string KeyName;
    public EContainerType containerType;
    public byte BaseSlots;
    public int CustomStackSize = 0;
    public CtxFlag AvailableTagItems = CtxFlag.All;
    public ContainerAvailabilityFlag AvailabilityFlag = ContainerAvailabilityFlag.FullAvailability;

    public EContainerType KeyOfCatalog
    {
        get => containerType;
        set => containerType = value;
    }
}