using UnityEngine;

public class PlayerIdleState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        Debug.Log("대기");
        
    }

    public void InputHandler()
    {
        float mouseY = Input.GetAxis("Mouse Y");
            
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            player.ChangeState(new PlayerMoveState());
        }
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded()) {
            player.ChangeState(new PlayerJumpState());
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.ChangeState(new PlayerAttackState());
        }
    }
    

    public void Update() {
        
    }

    public void PhysicsUpdate()
    {
        Quaternion targetRotation = Quaternion.Euler(0, player.cameraTransform.eulerAngles.y, 0);

        // 캐릭터가 카메라와 일치하는 방향으로 회전
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
    }

    public void Exit() { }
}