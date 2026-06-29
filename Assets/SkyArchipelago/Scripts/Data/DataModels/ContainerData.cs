using System.Collections.Generic;

[System.Serializable]
public class ContainerData : BaseData
{
    public int Slots;
    public List<ItemData> itemDatas = new();
}