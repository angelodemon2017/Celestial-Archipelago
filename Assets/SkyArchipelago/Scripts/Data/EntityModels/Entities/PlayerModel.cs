using UnityEngine;

public class PlayerModel : EntityModel<PlayerData>
{
    private Vector2 _currentMoveInput;

    public string PlayerId => GetModel.PlayerName;
    public override bool IsPhysical => true;
    public override float MoveSpeed => 5f;

    public PlayerModel(PlayerData data) : base(data)
    {
    }
}