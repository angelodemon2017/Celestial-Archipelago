using System;
using System.Collections.Generic;
using Zenject;

public class EntityRuntimeService : IInitializable, ITimeTickable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly SystemSO _systemSO;
    private readonly DataService _dataService;
    private readonly IGameTimeService _timeService;
    private readonly RecipesGlossaryService _recipesGlossaryService;

    private List<EntityModel> _entityModels = new(64);
    private Dictionary<int, EntityModel> _mapEntByIds = new(64);
    private int _lengthEntsUpdated = 0;
    private EntityModel[] _potentialUpdated = new EntityModel[1];
    private Dictionary<EEntityType, List<EntityModel>> _cacheEntsByType = new(64);
    private Dictionary<EEntityType, int> _cacheCount = new(64);

    private float _realTimer;

    public List<EntityModel> AllModels => _entityModels;

    public EntityRuntimeService(
        SignalBus signalBus,
        SystemSO systemSO,
        DataService dataService,
        IGameTimeService timeService,
        RecipesGlossaryService recipesGlossaryService)
    {
        _signalBus = signalBus;
        _systemSO = systemSO;
        _dataService = dataService;
        _timeService = timeService;
        _recipesGlossaryService = recipesGlossaryService;
    }

    public void Initialize()
    {
        _timeService.Register(this);
        _signalBus.Subscribe<ContainerOfEntityRequest>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<ContainerOfEntityRequest>(OnHandle);
        _timeService.Unregister(this);
    }

    private void OnHandle(ContainerOfEntityRequest containerOfEntityRequest)
    {
        if (_recipesGlossaryService.CurrentIdContainer == containerOfEntityRequest.IdContainer)
            _recipesGlossaryService.UpdateCurrentRecipes();
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _realTimer += gameDeltaTime;

        while (_realTimer > _systemSO.gameTick)
        {
            _realTimer -= _systemSO.gameTick;
            CheckAndRequestToViewUpdate();
        }
    }

    private void CheckAndRequestToViewUpdate()
    {
        ReadOnlySpan<EntityModel> span = new ReadOnlySpan<EntityModel>(_potentialUpdated, 0, _lengthEntsUpdated);
        for (int i = 0; i < _lengthEntsUpdated; i++)
        {
            var ent = span[i];
            if (ent.HaveChange)
            {
                _signalBus.Fire(new EntitiesUpdatedSignal(ent.Id, ent));
                ent.HaveChange = false;
            }
        }
    }

    public void RunWorld()
    {
        _realTimer = 0f;
        _entityModels.Clear();
        _cacheEntsByType.Clear();
        _cacheCount.Clear();

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
        _mapEntByIds[model.Id] = model;
        if (!_cacheEntsByType.TryGetValue(model.EntType, out List<EntityModel> list))
        {
            list = new List<EntityModel>(32);
            _cacheEntsByType[model.EntType] = list;
        }
        list.Add(model);
        _cacheCount[model.EntType] = list.Count;

        if (model.IsAvailableUpdate)
        {
            if (_lengthEntsUpdated >= _potentialUpdated.Length)
            {
                Array.Resize(ref _potentialUpdated, _potentialUpdated.Length * 2);
            }
            _potentialUpdated[_lengthEntsUpdated++] = model;
        }
    }

    public bool TryGetEntityById(int id, out EntityModel entity)
    {
        return _mapEntByIds.TryGetValue(id, out entity);
    }

    public List<EntityModel> GetModelsByEType(EEntityType etype)
    {
        if (!_cacheEntsByType.ContainsKey(etype))
        {
            _cacheEntsByType[etype] = new();
            _cacheCount[etype] = 0;
        }
        return _cacheEntsByType[etype];
    }

    public void Remove(EntityModel model)
    {
        _entityModels.Remove(model);
        _mapEntByIds.Remove(model.Id);
        var cacheList = GetModelsByEType(model.EntType);
        cacheList.Remove(model);
        _cacheCount[model.EntType] = cacheList.Count;

        for (int i = 0; i < _lengthEntsUpdated; i++)
        {
            if (_potentialUpdated[i] == model)
            {
                _potentialUpdated[i] = _potentialUpdated[--_lengthEntsUpdated];
                _potentialUpdated[_lengthEntsUpdated] = null;
            }
        }
        //need work with pool
    }
}