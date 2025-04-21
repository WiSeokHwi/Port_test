using UnityEngine;

public struct PlayerInputCommend
{
    public Vector2 MoveInput { get; }
    public bool JumpPressed { get; }
    public bool AttackPressed { get; }
    public bool RunPressed { get; }

    public bool IsGrounded { get; }
    
    public bool EquipPressed { get; }
    
    public bool ShiftTap { get; }

    public PlayerInputCommend(Vector2 move, bool jump, bool attack, bool run, bool isGrounded, bool equip, bool shift)
    {
        MoveInput = move;
        JumpPressed = jump;
        AttackPressed = attack;
        RunPressed = run;
        IsGrounded = isGrounded;
        EquipPressed = equip;
        ShiftTap = shift;
    }
}
