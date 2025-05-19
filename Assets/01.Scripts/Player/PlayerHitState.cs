using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitState : IPlayerState
{
    float time = 1f;
    PlayerController _player;
    public void Enter(PlayerController player)
    {
        _player = player;
        time = 1f;
    }

    public void HandleInput(PlayerInputCommend input)
    {
        
    }

    public void Update()
    {
        time -= Time.deltaTime;
        Debug.Log(time);
        if (time <= 0)
        {
            _player.ChangeState(new PlayerIdleState());
        }
    }

    public void PhysicsUpdate()
    {
        
    }

    public void Exit()
    {
        
    }


}
