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
    private int XMoveAnim;
    private int ZMoveAnim;
    
    private float speedLerp;
    private float speedLerpVelocity;
    
    private float lastShiftTapTime = -1f;
    private float doubleTapThreshold = 0.5f; // 0.3초 이내에 두번 누르면 구르기
    
    public void Enter(PlayerController player) {
        
        animator =player._animator;
        _player = player;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        XMoveAnim = Animator.StringToHash("XMove");
        ZMoveAnim = Animator.StringToHash("ZMove");
        
        
    }

    public void HandleInput(PlayerInputCommend input)
    {
        
        _input = input;
    }



    public void Update()
    {
        isRun = _input.RunPressed;
        
        if (_input.ShiftTap)
        {
            if (Time.time - lastShiftTapTime < doubleTapThreshold)
            {
                
                // 구르기 실행
                _player.ChangeState(new PlayerRollState());
                lastShiftTapTime = -1f; // 초기화
            }
            else
            {
                lastShiftTapTime = Time.time; // 첫 번째 탭 기록
            }
        }
        if (_input.JumpPressed && _input.IsGrounded)
        {
            animator.SetFloat(XMoveAnim, 0);
            animator.SetFloat(ZMoveAnim, 0);
            
            _player.ChangeState(new PlayerJumpState());
        }
        
        if ( _input.MoveInput.magnitude <= 0f && 
            Mathf.Abs(animX) <= 0.01f &&
            Mathf.Abs(animZ) <= 0.01f)
        {
            
            _player.ChangeState(new PlayerIdleState());
        }
        if (_input.AttackPressed && _player.equipped)
        {
            animator.SetFloat(XMoveAnim, 0);
            animator.SetFloat(ZMoveAnim, 0);
            _player.ChangeState(new PlayerAttackState());
        }

        if (_input.EquipPressed)
        {
            _player.WeaponEquip();
            
        }
    }

    public void PhysicsUpdate()
    {
        Move();
    }

    public void Exit() { }

    public void Move()
    {
        float x = _input.MoveInput.x;
        float z = _input.MoveInput.y;

        // 이동 애니메이션 업데이트
        UpdateAnimation(x, z);

        // 달리기 / 걷기 상태에 맞는 이동
        Vector3 moveDirection = GetMoveDirection(x, z);
        MovePlayer(moveDirection);
    }

    private void UpdateAnimation(float x, float z)
    {
        // 애니메이션 값 설정
        float targetSpeed = isRun ? 1f : 0.5f;
        
        speedLerp = Mathf.SmoothDamp(speedLerp, targetSpeed, ref speedLerpVelocity, 0.1f);
        
        float targetAnimX = speedLerp * x;
        float targetAnimZ = speedLerp * z;
        
        float smoothTime = (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f) ? 0.1f : 0.03f;

        // 스무딩 처리
        animX = Mathf.SmoothDamp(animX, targetAnimX, ref animXVelocity, smoothTime);
        animZ = Mathf.SmoothDamp(animZ, targetAnimZ, ref animZVelocity, smoothTime);

        animator.SetFloat(XMoveAnim, animX);
        animator.SetFloat(ZMoveAnim, animZ);
    }

    private Vector3 GetMoveDirection(float x, float z)
    {
        Vector3 camForward = _player.cameraTransform.forward;
        Vector3 camRight = _player.cameraTransform.right;
        
        // y축 기준으로 방향을 정리하고, 이동 방향 계산
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        return camForward * z + camRight * x;
    }

    private void MovePlayer(Vector3 moveDirection)
    {
        // 이동 처리
        float movementSpeed = moveSpeed * speedLerp;
        _player.rb.MovePosition(_player.rb.position + moveDirection * (movementSpeed * Time.fixedDeltaTime));

        // 회전 처리
        Quaternion targetRotation = Quaternion.Euler(0, _player.cameraTransform.eulerAngles.y, 0);
        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);

        _player.LastMoveDirection = moveDirection;
    }
}