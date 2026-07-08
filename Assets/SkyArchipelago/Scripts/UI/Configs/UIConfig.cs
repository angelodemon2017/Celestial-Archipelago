using UnityEngine;

[CreateAssetMenu(menuName = "UIConfig")]
public class UIConfig : ScriptableObject
{
    public Canvas _canvas;

    [Header("Views")]
    public MenuOfEntityWithInventoryView menuOfEntityWithInventory;
    public MainMenuControllerView mainMenuControllerView;
    public DialogMenuUI dialogMenuUI;
    public MenuOfManagerView menuOfManagerView;
    public GameplayControllerView gameplayControllerView;
    public InventoryView inventoryView;
    public PauseMenuUI pauseMenuUI;
}