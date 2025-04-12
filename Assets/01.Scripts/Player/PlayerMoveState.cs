using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    private PlayerController player;
    private float moveSpeed;
    private float runSpeed;
    private Vector3 movement;
    private bool isRun;
    private Animator animator;
    private float animX;
    private float animZ; 
    private float animXVelocity;
    private float animZVelocity;
    private int XmoveAnim;
    private int ZmoveAnim;
    private float targetAnimX;
    private float targetAnimZ;
    
    
    public void Enter(PlayerController player) {
        Debug.Log("이동");
        animator =player._animator;
        this.player = player;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        XmoveAnim = Animator.StringToHash("XMove");
        ZmoveAnim = Animator.StringToHash("ZMove");
        
    }

    public void InputHandler()
    {
        isRun = Input.GetKey(KeyCode.LeftShift);
        
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded()) player.ChangeState(new PlayerJumpState());
        
        if( player.rb.linearVelocity == Vector3.zero )
        {
            
            player.ChangeState(new PlayerIdleState());
        }
        
    }



    public void Update()
    {
        
    }

    public void PhysicsUpdate()
    {
        float x = Input.GetAxis("Horizontal"); // ← Raw 제거!
        float z = Input.GetAxis("Vertical");
        
        float targetSpeed = isRun ? 1f : 0.5f;

        targetAnimX = x == 0 ? 0 : x * targetSpeed;
        targetAnimZ = z == 0 ? 0 : z * targetSpeed;
        
        float smoothTime = (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f) ? 0.05f : 0.01f; // 움직일 땐 느리게, 멈출 땐 빠르게

        animX = Mathf.SmoothDamp(animX, targetAnimX, ref animXVelocity, smoothTime);
        animZ = Mathf.SmoothDamp(animZ, targetAnimZ, ref animZVelocity, smoothTime);
        
        Vector3 inputDirection = new Vector3(x, 0, z).normalized;
        Vector3 moveDirection = player.transform.TransformDirection(inputDirection);
        
        movement = isRun 
            ? moveDirection * (moveSpeed * runSpeed * Time.fixedDeltaTime) 
            : moveDirection * (moveSpeed * Time.fixedDeltaTime);

        
        
        animator.SetFloat(XmoveAnim, animX, 0.1f, Time.deltaTime);
        animator.SetFloat(ZmoveAnim, animZ, 0.1f, Time.deltaTime);
        
        
        player.Move(movement);
    }

    public void Exit() { }
}