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
        player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }


    public void InputHandler()
    {
        
    }

    public void Update() {
        
    }

    public void PhysicsUpdate()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jumping") && stateInfo.normalizedTime >= 1f && player.IsGrounded())
        {
            player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Exit() { }
}