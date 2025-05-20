
public class PlayerIdleState : IPlayerState {
    
    public override void Update() 
    {
        if (PlayerInput.MoveInput.magnitude > 0f)
        {
            Player.ChangeState(new PlayerMoveState());
        }
        else if (PlayerInput.JumpPressed && Player.controller.isGrounded)
        {
            Player.ChangeState(new PlayerJumpState());
        }

        else if (PlayerInput.AttackPressed && Player.equipped)
        {
            Player.ChangeState(new PlayerAttackState());
        }
        if (PlayerInput.EquipPressed)
        {
            Player.WeaponEquip();
            
        }
    }

}