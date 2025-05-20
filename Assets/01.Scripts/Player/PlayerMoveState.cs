using UnityEngine;
public class PlayerMoveState : IPlayerState {
    
    
    private float moveSpeed;
    private float runSpeed;
    private Vector3 movement;
    private bool isRun;
    private Animator animator;
    private float animX;
    private float animZ; 
    private float animXVelocity;
    private float animZVelocity;
    private int xMoveAnim;
    private int zMoveAnim;
    
    private float speedLerp;
    private float speedLerpVelocity;
    
    private float lastShiftTapTime = -1f;
    private float doubleTapThreshold = 0.5f; // 0.3초 이내에 두번 누르면 구르기
    
    public override void Enter(PlayerController player) 
    {
        base.Enter(player);
        animator =player._animator;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        xMoveAnim = Animator.StringToHash("XMove");
        zMoveAnim = Animator.StringToHash("ZMove");
    }



    public override void Update()
    {
        isRun = PlayerInput.RunPressed;
        
        Player.movePosition.x = Move().x;
        Player.movePosition.z = Move().z;
        
        if (PlayerInput.ShiftTap)
        {
            if (Time.time - lastShiftTapTime < doubleTapThreshold)
            {
                
                // 구르기 실행
                Player.ChangeState(new PlayerRollState());
                lastShiftTapTime = -1f; // 초기화
            }
            else
            {
                lastShiftTapTime = Time.time; // 첫 번째 탭 기록
            }
        }
        if (PlayerInput.JumpPressed && Player.controller.isGrounded)
        {
            animator.SetFloat(xMoveAnim, 0);
            animator.SetFloat(zMoveAnim, 0);
            
            Player.ChangeState(new PlayerJumpState());
        }
        
        if ( PlayerInput.MoveInput.magnitude <= 0f && 
             Mathf.Abs(animX) <= 0.01f &&
             Mathf.Abs(animZ) <= 0.01f)
        {
            
            Player.ChangeState(new PlayerIdleState());
        }
        if (PlayerInput.AttackPressed && Player.equipped)
        {
            animator.SetFloat(xMoveAnim, 0);
            animator.SetFloat(zMoveAnim, 0);
            Player.ChangeState(new PlayerAttackState());
        }

        if (PlayerInput.EquipPressed)
        {
            Player.WeaponEquip();
        }
    }
    
    public Vector3 Move()
    {
        float x = PlayerInput.MoveInput.x;
        float z = PlayerInput.MoveInput.y;

        // 이동 애니메이션 업데이트
        UpdateAnimation(x, z);

        // 달리기 / 걷기 상태에 맞는 이동
        Vector3 moveDirection = GetMoveDirection(x, z);
        return MovePlayer(moveDirection);
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

        animator.SetFloat(xMoveAnim, animX);
        animator.SetFloat(zMoveAnim, animZ);
    }

    private Vector3 GetMoveDirection(float x, float z)
    {
        Vector3 camForward = Player.cameraTransform.forward;
        Vector3 camRight = Player.cameraTransform.right;
        
        // y축 기준으로 방향을 정리하고, 이동 방향 계산
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        return camForward * z + camRight * x;
    }

    private Vector3 MovePlayer(Vector3 moveDirection)
    {
        // 이동 처리
        float movementSpeed = moveSpeed * speedLerp;
        return (moveDirection * movementSpeed);
    }
}