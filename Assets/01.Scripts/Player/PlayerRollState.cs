using UnityEngine;

public class PlayerRollState : IPlayerState
{

    private Animator animator;
    private int weaponLayerIndex;
    int rollTriggerHash = Animator.StringToHash("Roll");
    private int XMoveAnim;
    private int ZMoveAnim;

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.movePosition = Vector3.zero;
        animator = player._animator;
        weaponLayerIndex = animator.GetLayerIndex(player.CurrentWeapon.comboData.animationLayerName);
        
        XMoveAnim = Animator.StringToHash("XMove");
        ZMoveAnim = Animator.StringToHash("ZMove");
        animator.SetFloat(XMoveAnim, 0);
        animator.SetFloat(ZMoveAnim, 0);
        animator.applyRootMotion = true;
        animator.SetTrigger(rollTriggerHash);
    }


    public override void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);
        
        Quaternion targetRotation = Quaternion.Euler(0, Player.cameraTransform.eulerAngles.y, 0);
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        
        if (animator.IsInTransition(weaponLayerIndex)) return;
        
        if (stateInfo.normalizedTime >= 0.95f )
        {
            Player.ChangeState(new PlayerIdleState());
        }
    }

    public override void Exit()
    {
        animator.applyRootMotion = false;
    }

}
