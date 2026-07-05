using System.Collections.Generic;

[System.Serializable]
public class DataCollection<T, T2>
    where T : BaseData<T2>
    where T2 : BaseDataConfig
{
    public int MaxId;
    public List<int> AvailableIds = new();
    public List<T> Datas = new();

    private Dictionary<int, T> _cacheDatas = new();

    public int AddNewData(T newData)
    {
        if (AvailableIds.Count > 0)
        {
            newData.Id = AvailableIds[0];
            AvailableIds.RemoveAt(0);
        }
        else
            newData.Id = MaxId++;

        Datas.Add(newData);
        _cacheDatas.Add(newData.Id, newData);
        return newData.Id;
    }

    public void RemoveData(T data)
    {
        Datas.Remove(data);
        _cacheDatas.Remove(data.Id);
        AvailableIds.Add(data.Id);
    }

    public T GetElement(int id)
    {
        if (!_cacheDatas.ContainsKey(id))
        {
            _cacheDatas.Clear();
            Datas.ForEach(d => _cacheDatas.Add(d.Id, d));
        }

        if (_cacheDatas.TryGetValue(id, out T data))
            return data;

        return null;
    }
}