using UnityEngine;
using UnityEngine.Windows;

public class PlayerIdleState : IPlayerState {
    private PlayerController _player;
    private PlayerInputCommend _input;

    public void Enter(PlayerController player) {
        _player = player;
        Debug.Log("대기");
        
    }

    public void HandleInput(PlayerInputCommend input)
    {
        _input = input;
    }
    

    public void Update() {
        if (_input.MoveInput.magnitude > 0f)
        {
            _player.ChangeState(new PlayerMoveState());
        }
        else if (_input.JumpPressed && _input.IsGrounded)
        {
            _player.ChangeState(new PlayerJumpState());
        }

        else if (_input.AttackPressed && _player.equipped)
        {
            _player.ChangeState(new PlayerAttackState());
        }
        if (_input.EquipPressed)
        {
            _player.WeaponEquip();
            
        }
    }

    public void PhysicsUpdate()
    {

    }

    public void Exit() { }
}