using UnityEngine;

public class PlayerIdleState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.SetAnimation("Idle");
    }

    public void Update() {
        if (Input.GetAxisRaw("Horizontal") != 0) {
            player.ChangeState(new PlayerMoveState());
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            player.ChangeState(new PlayerJumpState());
        }
    }

    public void Exit() { }
}