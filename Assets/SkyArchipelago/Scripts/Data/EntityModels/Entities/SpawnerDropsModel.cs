using System.Collections.Generic;

public class SpawnerDropsModel : EntityModel<SpawnerDropsData>
{
    public int Falls { get; set; }
    public float CurrentChance { get; set; }
    public int CurrentCountDrops
    {
        get => GetData.CurrentCountDrops;
        set => GetData.CurrentCountDrops = value;
    }
    public List<int> OwnItems => GetData.OwnItems;

    public SpawnerDropsModel(SpawnerDropsData data) : base(data)
    {
    }
}