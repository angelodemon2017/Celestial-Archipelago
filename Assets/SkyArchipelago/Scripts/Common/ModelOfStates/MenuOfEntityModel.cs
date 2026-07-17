using System;

public class MenuOfEntityModel
{
    private EntityModel _selectedModel;

    public EntityModel EntMod => _selectedModel;

    public Action OnTryClosed;

    public void SetEM(EntityModel selectedModel)
    {
        _selectedModel = selectedModel;
    }

    public void CleanByExitState()
    {
        _selectedModel = null;
    }
}