
public interface IPlayerState
{
    
    void Enter(PlayerController player);
    void HandleInput(PlayerInputCommend input);
    void Update();

    void PhysicsUpdate();
    void Exit();
}