
public interface IPlayerState {
    void Enter(PlayerController player);
    void InputHandler();
    void Update();

    void PhysicsUpdate();
    void Exit();
}