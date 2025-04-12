using UnityEngine;

public class PlayerIdleState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        Debug.Log("대기");
        
    }

    public void InputHandler()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            player.ChangeState(new PlayerMoveState());
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            player.ChangeState(new PlayerJumpState());
        }
    }
    

    public void Update() {
        
    }

    public void PhysicsUpdate()
    {
        
    }

    public void Exit() { }
}