using UnityEngine;

public class PlayerFallState : IPlayerState
{
    public override void Update()
    {
        if (Player.controller.isGrounded && Player.controller.velocity.y < 0f)
        {
            Player.ChangeState(new PlayerLandState());
        }
    }
}
