using UnityEngine;

public class PlayerJumpState : IPlayerState {

    private Animator animator;
    private AnimatorStateInfo stateInfo;
    
    private static readonly int JumpTrigger = Animator.StringToHash("Jump");
    
    private int WeaponLayer;
    

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        animator = player._animator;
        // 점프 애니메이션 재생
        WeaponLayer = animator.GetLayerIndex(player.CurrentWeapon.comboData.animationLayerName);
        stateInfo = animator.GetCurrentAnimatorStateInfo(WeaponLayer);
        animator.SetTrigger(JumpTrigger);
        
    }

    public override void Update() 
    {
        if (Player.movePosition.y > 0f)
        {
            Player.ChangeState(new PlayerFallState());
        }
    }

}