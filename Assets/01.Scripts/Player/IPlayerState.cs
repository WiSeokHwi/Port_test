
public abstract class IPlayerState
{
    protected PlayerController Player;
    protected PlayerInputCommend PlayerInput;

    public void HandleInput(PlayerInputCommend input)
    {
        PlayerInput = input;
    }
    public virtual void Enter(PlayerController player)
    {
        this.Player = player;
    }

    public virtual void Update()
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void Exit()
    {
        
    }
}