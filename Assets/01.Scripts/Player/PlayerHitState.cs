using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitState : IPlayerState
{
    float time = 1f;


    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.movePosition = Vector3.zero;
        player._animator.SetTrigger("Hit");
        time = 1f;
    }

    public override void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Player.ChangeState(new PlayerIdleState());
        }
    }

}
