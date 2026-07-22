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
    private readonly ContainersService _containersService;
    private readonly CraftProcessRepository _craftProcessRepository;

    private Dictionary<EEntityType, Queue<EntityModel>> _entityPools = new();

    private List<EntityModel> _entityModels = new(64);
    private Dictionary<int, EntityModel> _mapEntByIds = new(64);
    private int _lengthEntsUpdated = 0;
    private EntityModel[] _potentialUpdated = new EntityModel[1];
    private Dictionary<EEntityType, List<EntityModel>> _cacheEntsByType = new(64);
    private Dictionary<EEntityType, int> _cacheCount = new(64);

    private float _realTimer;

    public List<EntityModel> AllModels => _entityModels;

    public Action<EntityModel, int> BeforeDeleteEntity;

    public EntityRuntimeService(
        SignalBus signalBus,
        SystemSO systemSO,
        DataService dataService,
        IGameTimeService timeService,
        RecipesGlossaryService recipesGlossaryService,
        ContainersService containersService,
        CraftProcessRepository craftProcessRepository)
    {
        _signalBus = signalBus;
        _systemSO = systemSO;
        _dataService = dataService;
        _timeService = timeService;
        _recipesGlossaryService = recipesGlossaryService;
        _containersService = containersService;
        _craftProcessRepository = craftProcessRepository;
    }

    public void Initialize()
    {
        _timeService.Register(this);
        _signalBus.Subscribe<ContainerOfEntityRequest>(OnHandle);
        _signalBus.Subscribe<EntityDeleteRequestSignal>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<ContainerOfEntityRequest>(OnHandle);
        _signalBus?.Unsubscribe<EntityDeleteRequestSignal>(OnHandle);
        _timeService.Unregister(this);
    }

    private void OnHandle(ContainerOfEntityRequest containerOfEntityRequest)
    {
        if (_recipesGlossaryService.CurrentIdContainer == containerOfEntityRequest.IdContainer)
            _recipesGlossaryService.UpdateCurrentItemRecipes();
    }

    private void OnHandle(EntityDeleteRequestSignal entityDeleteRequest)
    {
        if (_mapEntByIds.TryGetValue(entityDeleteRequest.IdEntity, out var entity))
            Remove(entity);
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
                var model = CreateEntityModel(entity);
                AddModel(model);
            }
        }
    }

    public EntityModel CreateEntityModel(EntityData entityData)
    {
        var pool = GetPool(entityData.EntityType);
        if (pool.Count > 0)
        {
            var entity = pool.Dequeue();
            entity.OnSpawned(entityData);
            return entity;
        }
        return entityData.CreateModel();
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
        _craftProcessRepository.DeletedEntity(model);
        _containersService.DeletedEntity(model);
        BeforeDeleteEntity?.Invoke(model, model.IdOwner);
        _dataService.worldData.StaticIslands.Datas[0].entities.RemoveData(model.Id);
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

        var pool = GetPool(model.EntType);
        pool.Enqueue(model);
        model.OnDespawned();
    }

    private Queue<EntityModel> GetPool(EEntityType entityType)
    {
        if (!_entityPools.ContainsKey(entityType))
            _entityPools[entityType] = new();
        return _entityPools[entityType];
    }
}