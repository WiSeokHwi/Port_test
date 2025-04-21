using UnityEngine;

public class PlayerJumpState : IPlayerState {
    private PlayerController player;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private PlayerInputCommend _input;
    
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");

    private int WeaponLayer;

    public void Enter(PlayerController player) {
        
        Debug.Log("점프!");
        this.player = player;
        
        animator = player._animator;
        // 점프 애니메이션 재생
        WeaponLayer = animator.GetLayerIndex(player.CurrentWeapon.comboData.animationLayerName);
        stateInfo = animator.GetCurrentAnimatorStateInfo(WeaponLayer);
        animator.SetTrigger(JumpTrigger);
        player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }


    public void HandleInput(PlayerInputCommend input)
    {
        _input = input;

    }

    public void Update() 
    {
        if (_input.IsGrounded && stateInfo.normalizedTime >= 0.9f)
        {
            player.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void PhysicsUpdate()
    {
        
    }

    public void Exit() { }
}