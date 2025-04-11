using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    private PlayerController player;
    private float moveSpeed;
    private float runSpeed;
    private Vector3 movement;
    private bool isRun = false;
    
    public void Enter(PlayerController player) {
        Debug.Log("이동");
        this.player = player;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        
    }

    public void InputHandler()
    {
        isRun = Input.GetKey(KeyCode.LeftShift);
        
        if (Input.GetKeyDown(KeyCode.Space)) player.ChangeState(new PlayerJumpState());
        
        if( Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 ) 
            player.ChangeState(new PlayerIdleState());
        
    }



    public void Update()
    {
        
    }

    public void PhysicsUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 moveDirection = new Vector3(h, 0, v).normalized;
        Debug.Log(moveDirection);
        movement = isRun 
            ? moveDirection * (moveSpeed * runSpeed) 
            : moveDirection * (moveSpeed);

        Debug.Log(movement);
        
        player.Move(movement);
    }

    public void Exit() { }
}