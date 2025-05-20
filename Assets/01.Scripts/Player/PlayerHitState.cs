using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitState : IPlayerState
{
    float time = 1f;


    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        time = 1f;
    }

    public override void Update()
    {
        time -= Time.deltaTime;
        Debug.Log(time);
        if (time <= 0)
        {
            Player.ChangeState(new PlayerIdleState());
        }
    }

}
