using System;
using System.Collections.Generic;

public class EntityRuntimeService : ITimeTickable, IDisposable
{
    private readonly DataService _dataService;
    private readonly IGameTimeService _timeService;

    private List<EntityModel> _entityModels = new(64);
//    private Dictionary<Type, List<EntityModel>> _cacheByBase = new(64);
    private Dictionary<EEntityType, List<EntityModel>> _cacheEntsByType = new(64);

    private float _realTimer;

    public List<EntityModel> AllModels => _entityModels;

    public EntityRuntimeService(
        DataService dataService,
        IGameTimeService timeService)
    {
        _dataService = dataService;
        _timeService = timeService;

        _timeService.Register(this);
    }

    public void Dispose()
    {
        _timeService.Unregister(this);
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _realTimer += gameDeltaTime;

        while (_realTimer > 1f)
        {
            _realTimer -= 1f;
        }
    }

    public void RunWorld()
    {
        _realTimer = 0f;
        _entityModels.Clear();
//        _cacheByBase.Clear();
        _cacheEntsByType.Clear();

        foreach (var isl in _dataService.GetAllIslands)
        {
            foreach (var entity in isl.entities.Datas)
            {
                var model = entity.CreateModel();
                AddModel(model);
            }
        }
    }

    public void AddModel(EntityModel model)
    {
        _entityModels.Add(model);
//        var type = model.GetType();
/*        if (!_cacheByBase.TryGetValue(type, out var list))
        {
            list = new List<EntityModel>(32);
            _cacheByBase[type] = list;
        }/**/
        if (!_cacheEntsByType.TryGetValue(model.EntType, out List<EntityModel> list))
        {
            list = new List<EntityModel>(32);
            _cacheEntsByType[model.EntType] = list;
        }
        list.Add(model);
    }

/*    public List<T> GetModelsByType<T>() where T : EntityModel
    {
        if (_cacheByBase.TryGetValue(typeof(T), out var list))
        {
            return Unsafe.As<List<EntityModel>, List<T>>(ref list!);
        }
        return new List<T>();
    }/**/

    public List<EntityModel> GetModelsByEType(EEntityType etype)
    {
        if (_cacheEntsByType.TryGetValue(etype, out var list))
        {
            return list;
        }
        return new List<EntityModel>();
    }
}