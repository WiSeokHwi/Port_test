using UnityEngine;

public class PlayerLandState : IPlayerState
{
    public override void Update()
    {
        Player.ChangeState(new PlayerIdleState());
    }
}
