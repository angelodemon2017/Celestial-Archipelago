using UnityEngine;

public class FPSCommonModel
{
    public float JumpForce = 5f;

    public string PlayerName = "Player";

    public PlayerModel LocalPlayerModel;
    public EntityViewMB LocalPlayerView;
    
    public float XRotation = 0f;
    public Vector2 CurrentMoveInput;
}