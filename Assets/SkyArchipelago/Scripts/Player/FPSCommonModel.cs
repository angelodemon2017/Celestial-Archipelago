using UnityEngine;

public class FPSCommonModel
{
    private readonly ContainersService _containersService;

    public float JumpForce = 5f;

    public string PlayerName = "Player";

    public PlayerModel LocalPlayerModel;
    public ContainerModel ContainerModel;

    public float XRotation = 0f;
    public Vector2 CurrentMoveInput;

    public FPSCommonModel(
        ContainersService containersService)
    {
        _containersService = containersService;
    }

    public void SetPlModel(PlayerModel playerModel)
    {
        LocalPlayerModel = playerModel;
        ContainerModel = _containersService.GetContainerModel(LocalPlayerModel);
    }
}