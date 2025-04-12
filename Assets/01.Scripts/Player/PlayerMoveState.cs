using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    private PlayerController player;
    private float moveSpeed;
    private float runSpeed;
    private Vector3 movement;
    private bool isRun = false;
    private Animator animator;
    private float animX;
    private float animZ; 
    float acceleration = 3f;
    float deceleration = 6f;
    
    public void Enter(PlayerController player) {
        Debug.Log("이동");
        animator =player._animator;
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
        float x = Input.GetAxis("Horizontal"); // ← Raw 제거!
        float z = Input.GetAxis("Vertical");
        
        float targetSpeed = isRun ? 1f : 0.5f;

        animX = Mathf.MoveTowards(animX, x * targetSpeed, ((Mathf.Abs(x) > 0.01f) ? acceleration : deceleration) * Time.fixedDeltaTime);
        animZ = Mathf.MoveTowards(animZ, z * targetSpeed, ((Mathf.Abs(z) > 0.01f) ? acceleration : deceleration) * Time.fixedDeltaTime);
        
        float moveX = Mathf.MoveTowards(animX, x , ((Mathf.Abs(x) > 0.01f) ? acceleration : deceleration) * Time.fixedDeltaTime);
        float moveZ = Mathf.MoveTowards(animZ, z , ((Mathf.Abs(z) > 0.01f) ? acceleration : deceleration) * Time.fixedDeltaTime);
        
        
        
        Vector3 inputDirection = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 moveDirection = player.transform.TransformDirection(inputDirection);
        
        
        
        Debug.Log(moveDirection);
        Debug.Log(inputDirection);
        movement = isRun 
            ? moveDirection * (moveSpeed * runSpeed * Time.fixedDeltaTime) 
            : moveDirection * (moveSpeed * Time.fixedDeltaTime);

        Debug.Log(movement);
        
        animator.SetFloat("XMove", animX, 0.1f, Time.deltaTime);
        animator.SetFloat("ZMove", animZ, 0.1f, Time.deltaTime);
        
        player.Move(movement);
    }

    public void Exit() { }
}