public class PlayerJumpState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.SetAnimation("Jump");
        player.Jump();
    }

    public void Update() {
        if (player.IsGrounded()) {
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit() { }
}