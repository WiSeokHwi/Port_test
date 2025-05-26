using UnityEngine;

public struct PlayerInputCommend
{
    public Vector2 MoveInput { get; }
    
    public Vector2 RotationInput { get; }
    public bool JumpPressed { get; }
    public bool AttackPressed { get; }
    public bool RunPressed { get; }
    
    public bool EquipPressed { get; }
    
    public bool rollTriggerPressed { get; }
    


    public PlayerInputCommend(Vector2 move,Vector2 rotation, bool jump, bool attack, bool run, bool equip, bool roll)
    {
        MoveInput = move;
        RotationInput = rotation;
        JumpPressed = jump;
        AttackPressed = attack;
        RunPressed = run;
        EquipPressed = equip;
        rollTriggerPressed = roll;
    }
}
