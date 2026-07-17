using System;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class DataCollection<T, T2> : BaseData<T2>
    where T : BaseData<T2>
    where T2 : BaseDataConfig
{
    private int _maxId;
    private int _countAvailable;
    public List<int> AvailableIds = new();
    private int _countData;
    public List<T> Datas = new();

    [NonSerialized]
    private Dictionary<int, T> _cacheDatas = new();

    public int AddNewData(T newData)
    {
        if (AvailableIds.Count > 0)
        {
            newData.Id = AvailableIds[0];
            AvailableIds.RemoveAt(0);
            _countAvailable--;
        }
        else
            newData.Id = _maxId++;

        Datas.Add(newData);
        _countData++;
        _cacheDatas.Add(newData.Id, newData);
        return newData.Id;
    }

    public void RemoveData(int id)
    {
        if (_cacheDatas.TryGetValue(id, out T result))
            RemoveData(result);
    }

    private void RemoveData(T data)
    {
        Datas.Remove(data);
        _countData--;
        _cacheDatas.Remove(data.Id);
        AvailableIds.Add(data.Id);
        _countAvailable++;
    }

    public bool TryGetElement(int id, out T element)
    {
        return _cacheDatas.TryGetValue(id, out element);
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

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        _maxId = binaryReader.ReadInt32();
        _countAvailable = binaryReader.ReadInt32();
        for (int i = 0; i < _countAvailable; i++)
            AvailableIds.Add(binaryReader.ReadInt32());
        _countData = binaryReader.ReadInt32();
        //        for (int i = 0; i < _countData; i++)
        //            Datas.Add();//??
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(_maxId);
        writer.Write(_countAvailable);
        for (int i = 0; i < _countAvailable; i++)
            writer.Write(AvailableIds[i]);
        writer.Write(_countData);
        for (int i = 0; i < _countData; i++)
            Datas[i].SaveToBinary(writer);
    }
}