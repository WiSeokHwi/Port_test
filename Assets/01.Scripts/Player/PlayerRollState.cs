using UnityEngine;

public class PlayerRollState : IPlayerState
{
   

    private Animator animator;
    private int weaponLayerIndex;
    int rollTriggerHash = Animator.StringToHash("Roll");


    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        player.movePosition = Vector3.zero;
        
        animator = player._animator;
        weaponLayerIndex = animator.GetLayerIndex(player.CurrentWeapon.comboData.animationLayerName);

        // 마지막 이동 방향 가져오기 (없으면 전방 유지)
        Vector3 rollDir = player.LastMoveDirection.magnitude > 0.01f
            ? player.LastMoveDirection.normalized
            : player.transform.forward;

        // 회전 방향 설정 (애니메이션은 항상 forward로 이동하므로)
        if (rollDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rollDir, Vector3.up);
            player.transform.rotation = targetRotation;
        }
        
        animator.applyRootMotion = true;
        animator.SetTrigger(rollTriggerHash);
    }


    public override void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);
        
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
