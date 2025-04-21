using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    private PlayerController _player;
    private PlayerInputCommend _input;

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
        _player = player;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        XmoveAnim = Animator.StringToHash("XMove");
        ZmoveAnim = Animator.StringToHash("ZMove");
        
    }

    public void HandleInput(PlayerInputCommend input)
    {
        
        _input = input;
    }



    public void Update()
    {
        if (_input.JumpPressed && _input.IsGrounded)
        {
            animator.SetFloat(XmoveAnim, 0);
            animator.SetFloat(ZmoveAnim, 0);
            
            _player.ChangeState(new PlayerJumpState());
        }
        
        if( _input.MoveInput.magnitude <= 0f )
        {
            
            _player.ChangeState(new PlayerIdleState());
        }
        if (_input.AttackPressed)
        {
            animator.SetFloat(XmoveAnim, 0);
            animator.SetFloat(ZmoveAnim, 0);
            _player.ChangeState(new PlayerAttackState());
        }
    }

    public void PhysicsUpdate()
    {
        isRun = _input.RunPressed;
        float x = _input.MoveInput.x;
        float z = _input.MoveInput.y;
        
        float targetSpeed = isRun ? 1f : 0.5f;

        targetAnimX = x == 0 ? 0 : x * targetSpeed;
        targetAnimZ = z == 0 ? 0 : z * targetSpeed;
        
        float smoothTime = (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f) ? 0.05f : 0.01f; // 움직일 땐 느리게, 멈출 땐 빠르게

        animX = Mathf.SmoothDamp(animX, targetAnimX, ref animXVelocity, smoothTime);
        animZ = Mathf.SmoothDamp(animZ, targetAnimZ, ref animZVelocity, smoothTime);
        
        Vector3 camForward = _player.cameraTransform.forward;
        Vector3 camRight = _player.cameraTransform.right;

// y축을 기준으로만 방향을 잡기 위해 수직 방향 제거
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = new Vector3(x, 0, z);
        Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
        
        movement = isRun 
            ? moveDirection * (moveSpeed * runSpeed * Time.fixedDeltaTime) 
            : moveDirection * (moveSpeed * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, _player.cameraTransform.eulerAngles.y, 0);

        // 캐릭터가 카메라와 일치하는 방향으로 회전
        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);



        
        animator.SetFloat(XmoveAnim, animX, 0.1f, Time.deltaTime);
        animator.SetFloat(ZmoveAnim, animZ, 0.1f, Time.deltaTime);
        
        
        _player.Move(movement);
    }

    public void Exit() { }

}