using UnityEngine;

public class PlayerLandState : IPlayerState
{
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        Player.movePosition = Vector3.zero;
    }

    public override void Update()
    {
        Player.ChangeState(new PlayerIdleState());
    }
}
