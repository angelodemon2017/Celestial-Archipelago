using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Core/UIConfig")]
public class UIConfig : ScriptableObject
{
    public Canvas _canvas;

    [Header("Views")]
    public MenuOfEntityView _menuOfEntityView;
    public MainMenuControllerView mainMenuControllerView;
    public DialogMenuUI dialogMenuUI;
    public MenuOfManagerView menuOfManagerView;
    public GameplayControllerView gameplayControllerView;
    public InventoryView inventoryView;
    public PauseMenuUI pauseMenuUI;

    [Header("PartOfWindows")]    
    [SerializeField] private ListOfKeyHintsMB _listOfKeyHints;
    [SerializeField] private ItemsListViewMB _itemsListViewMB;
    [SerializeField] private FurnaceViewMB _furnaceView;
    [SerializeField] private SimpleChestViewMB _simpleChestView;
    [SerializeField] private SimpleWorkbenchViewMB _simpleWorkbenchView;
    
    [Header("Small elements")]
    [SerializeField] private TextLabelOfVerticalbarMB _textLabelOfVerticalbar;
    [SerializeField] private IconRecipeViewMB _iconRecipeView;
    [SerializeField] private CostElementMB _costElement;
    [SerializeField] private IconViewMB _iconView;

    public void InstallPrefabs(DiContainer container)
    {
        InstallSmallElements(container);
        InstallPartOfViews(container);
        InstallViewOfStates(container);
    }

    private void InstallViewOfStates(DiContainer container)
    {        
        container.Bind<MenuOfEntityView>().FromComponentInNewPrefab(_menuOfEntityView).AsSingle();
        container.Bind<MenuOfManagerView>().FromComponentInNewPrefab(menuOfManagerView).AsSingle();
        container.Bind<GameplayControllerView>().FromComponentInNewPrefab(gameplayControllerView).AsSingle();
        container.Bind<InventoryView>().FromComponentInNewPrefab(inventoryView).AsSingle();
        container.Bind<MainMenuControllerView>().FromComponentInNewPrefab(mainMenuControllerView).AsSingle();
        container.Bind<DialogMenuUI>().FromComponentInNewPrefab(dialogMenuUI).AsSingle();
        container.Bind<PauseMenuUI>().FromComponentInNewPrefab(pauseMenuUI).AsSingle();
    }

    private void InstallPartOfViews(DiContainer container)
    {
        container.Bind<ListOfKeyHintsMB>().FromComponentInNewPrefab(_listOfKeyHints).AsTransient();
        container.Bind<ItemsListViewMB>().FromComponentInNewPrefab(_itemsListViewMB).AsTransient();
        container.Bind<FurnaceViewMB>().FromComponentInNewPrefab(_furnaceView).AsSingle();
        container.Bind<SimpleChestViewMB>().FromComponentInNewPrefab(_simpleChestView).AsSingle();
        container.Bind<SimpleWorkbenchViewMB>().FromComponentInNewPrefab(_simpleWorkbenchView).AsSingle();
        
        container.Bind<UIMBFactory<List<string>, ListOfKeyHintsMB>>().AsSingle();
        container.Bind<UIMBFactory<ContainerModel, ItemsListViewMB>>().AsSingle();
    }

    private void InstallSmallElements(DiContainer container)
    {
        container.Bind<TextLabelOfVerticalbarMB>().FromComponentInNewPrefab(_textLabelOfVerticalbar).AsTransient();
        container.Bind<IconViewMB>().FromComponentInNewPrefab(_iconView).AsTransient();
        container.Bind<IconRecipeViewMB>().FromComponentInNewPrefab(_iconRecipeView).AsTransient();
        container.Bind<CostElementMB>().FromComponentInNewPrefab(_costElement).AsTransient();

        container.Bind<UIMBFactory<string, TextLabelOfVerticalbarMB>>().AsSingle();
        container.Bind<UIMBFactory<ItemModel, IconViewMB>>().AsSingle();
        container.Bind<UIMBFactory<IModelOfRecipeElement, IconRecipeViewMB>>().AsSingle();
        container.Bind<UIMBFactory<IModelOfCostElement, CostElementMB>>().AsSingle();
    }
}