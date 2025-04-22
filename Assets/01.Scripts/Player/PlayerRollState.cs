using UnityEngine;

public class PlayerRollState : IPlayerState
{
    PlayerInputCommend _input;
    
    PlayerController _player;
    private Animator animator;
    private int weaponLayerIndex;
    int rollTriggerHash = Animator.StringToHash("Roll");
    private int XMoveAnim;
    private int ZMoveAnim;

    public void Enter(PlayerController player)
    {
        
        _player = player;
        animator = player._animator;
        weaponLayerIndex = animator.GetLayerIndex(_player.CurrentWeapon.comboData.animationLayerName);
        
        XMoveAnim = Animator.StringToHash("XMove");
        ZMoveAnim = Animator.StringToHash("ZMove");
        animator.SetFloat(XMoveAnim, 0);
        animator.SetFloat(ZMoveAnim, 0);
        animator.applyRootMotion = true;
        animator.SetTrigger(rollTriggerHash);
    }

    public void HandleInput(PlayerInputCommend input)
    {
        _input = input;
    }

    public void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(weaponLayerIndex);
        
        Quaternion targetRotation = Quaternion.Euler(0, _player.cameraTransform.eulerAngles.y, 0);
        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        
        if (animator.IsInTransition(weaponLayerIndex)) return;
        
        if (stateInfo.normalizedTime >= 0.95f )
        {
            _player.ChangeState(new PlayerIdleState());
        }
    }

    public void PhysicsUpdate()
    {
        
    }

    public void Exit()
    {
        animator.applyRootMotion = false;
    }

}
