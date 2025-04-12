using UnityEngine;

public class PlayerJumpState : IPlayerState {
    private PlayerController player;
    private Animator animator;
    
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");

    public void Enter(PlayerController player) {
        Debug.Log("점프!");
        this.player = player;
        animator = player._animator;
        // 점프 애니메이션 재생
        animator.SetTrigger(JumpTrigger);

    }


    public void InputHandler()
    {
        
    }

    public void Update() {
        
    }

    public void PhysicsUpdate()
    {

        player.ChangeState(new PlayerIdleState()); // 땅에 닿으면 Idle로 전환
    
    }

    public void Exit() { }
}