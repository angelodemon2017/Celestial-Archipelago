using UnityEngine;

public class FPSCommonModel
{
    private readonly ContainersService _containersService;

    public FPSCommonModel(
        ContainersService containersService)
    {
        _containersService = containersService;
    }

    public float JumpForce = 5f;

    public string PlayerName = "Player";

    public PlayerModel LocalPlayerModel;
    public ContainerModel ContainerModel;
    //    public EntityViewMB LocalPlayerView;

    public float XRotation = 0f;
    public Vector2 CurrentMoveInput;

    public void SetPlModel(PlayerModel playerModel)
    {
        LocalPlayerModel = playerModel;
        ContainerModel = _containersService.GetContainerModel(LocalPlayerModel);
    }
}