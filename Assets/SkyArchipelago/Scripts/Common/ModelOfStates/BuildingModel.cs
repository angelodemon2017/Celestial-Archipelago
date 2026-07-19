using System;
using Zenject;

public class BuildingModel : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly EntityRecipeCatalogManager _entityRecipeCatalogManager;

    public int RecipeId;
    public int IdEntity;

    public Action SelectedStruct;

    public BuildingModel(
        SignalBus signalBus,
        EntityRecipeCatalogManager entityRecipeCatalogManager)
    {
        _signalBus = signalBus;
        _entityRecipeCatalogManager = entityRecipeCatalogManager;
    }

    private void OnHandle(StartPlaceEntity startPlaceEntity)
    {
        if (_entityRecipeCatalogManager.TryGetConfigByKey(startPlaceEntity.RecipeId, out var recipe))
        {
            RecipeId = startPlaceEntity.RecipeId;
            IdEntity = recipe.EntityConfig.Uid;
            SelectedStruct?.Invoke();
        }
    }

    public void Initialize()
    {
        _signalBus.Subscribe<StartPlaceEntity>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<StartPlaceEntity>(OnHandle);
    }
}