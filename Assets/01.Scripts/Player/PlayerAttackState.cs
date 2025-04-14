using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;
    private Animator animator;
    private bool isComboAttack;
    
    public void Enter(PlayerController player)
    {
        animator =player._animator;
        this.player = player;
        
        player._animator.SetTrigger("Attack");
        isComboAttack = false;  // 콤보 상태 초기화
    }

    public void InputHandler()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션이 끝날 때까지는 Idle로 넘어가지 않도록
        if (stateInfo.normalizedTime < 1f && stateInfo.IsTag("Attack"))
        {
            return; // 애니메이션이 끝나지 않았으면 아무 작업도 하지 않음
        }

        // 공격 중 마우스 클릭을 다시 했을 경우 다음 공격으로 넘어가게 처리
        if (Input.GetMouseButtonDown(0) && !isComboAttack)
        {
            player._animator.SetTrigger("Attack");
            isComboAttack = true; // 콤보 공격이 시작됨
        }
        else if (stateInfo.normalizedTime >= 1f && stateInfo.IsTag("Attack") && !isComboAttack)
        {
            // 공격이 끝났을 때 Idle로 전환 (콤보 공격이 아닌 경우)
            player.ChangeState(new PlayerIdleState());
        }
    }

    public void Update()
    {
        
    }

    public void PhysicsUpdate()
    {
        
    }

    public void Exit()
    {
        isComboAttack = false;
    }
}
