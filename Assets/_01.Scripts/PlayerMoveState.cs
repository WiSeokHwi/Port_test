using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.SetAnimation("Run");
    }

    public void Update() {
        float h = Input.GetAxisRaw("Horizontal");
        player.Move(h);

        if (h == 0) {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit() { }
}