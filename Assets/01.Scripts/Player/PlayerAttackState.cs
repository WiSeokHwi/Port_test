using UnityEngine;

public class PlayerAttackState : IPlayerState
{
    private PlayerController player;
    private Animator animator;
    public void Enter(PlayerController player)
    {
        animator =player._animator;
        this.player = player;
        
        player._animator.SetTrigger("Attack");
    }

    public void InputHandler()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (Input.GetMouseButtonDown(0))
        {
            player._animator.SetTrigger("Attack");
        }
        
        else if (stateInfo.normalizedTime >= 1f && stateInfo.IsTag("Attack"))
        {
            
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
        
    }
}
