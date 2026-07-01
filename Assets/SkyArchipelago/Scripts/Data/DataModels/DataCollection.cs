using System.Collections.Generic;

[System.Serializable]
public class DataCollection<T>
    where T : BaseData
{
    public int MaxId;
    public List<int> AvailableIds = new();
    public List<T> Datas = new();

    public int AddNewData(T newData)
    {
        newData.Id = MaxId++;
        Datas.Add(newData);
        return newData.Id;
    }
}