using UnityEngine;

// 플레이어의 걷기/달리기 상태를 담당하는 상태 클래스
public class PlayerMoveState : IPlayerState {
    
    private float moveSpeed; // 기본 이동 속도
    private float runSpeed;  // 달리기 속도

    private Vector3 currentVelocity; // 현재 이동 속도 (이전 프레임 값 포함)
    private Vector3 moveVelocity;    // SmoothDamp용 내부 속도 보간 값 (ref로 넘겨줘야 함)

    private Animator animator; // 애니메이터 컴포넌트

    private float animX, animZ; // 애니메이터 파라미터용 x,z 방향값
    private float animXVelocity, animZVelocity; // 애니메이션 X, Z 보간용 속도 값
    private float animSpeedVelocity; // 달리기/걷기 애니메이션 속도 보간용 값

    private int xMoveAnim, zMoveAnim; // 애니메이터 파라미터 해시값

    private float speedLerp; // 걷기/달리기 보간된 속도값 (0.5~1.0 사이)

    private float lastShiftTapTime = -1f; // 마지막 Shift 입력 시간
    private float doubleTapThreshold = 0.5f; // 더블탭 인식 시간 간격

    // 상태 진입 시 실행 (초기화)
    public override void Enter(PlayerController player) {
        base.Enter(player);
        animator = player._animator;
        moveSpeed = player.moveSpeed;
        runSpeed = player.runSpeed;
        xMoveAnim = Animator.StringToHash("XMove"); // 애니메이션 파라미터 해시화
        zMoveAnim = Animator.StringToHash("ZMove");
    }

    // 상태 갱신 (매 프레임 호출)
    public override void Update() {
        bool isRun = PlayerInput.RunPressed; // 달리기 입력 확인
        Vector2 input = PlayerInput.MoveInput; // 이동 입력값 (WASD 등)
        
        Vector3 moveDirection = GetMoveDirection(input); // 카메라 기준 이동 방향 계산
        
        Player.LastMoveDirection = moveDirection.normalized;
        // 회전 처리
        Vector3 cameraForward = Player.cameraTransform.forward;
        
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        
        Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, 3f * Time.deltaTime);
        
        float targetSpeed = (isRun ? runSpeed : moveSpeed) * input.magnitude; // 입력 세기에 따라 속도 결정
        

        // 부드러운 이동 속도 보간
        Vector3 targetVelocity = moveDirection * targetSpeed;
        // 매 프레임 currentVelocity 갱신 하여 targetVelocity 과 속도 보간
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref moveVelocity, 0.1f);
        
        // 애니메이션 업데이트
        UpdateAnimation(input, isRun);
        // 최종 위치 이동값 적용 (Y값은 기존 값 유지)
        Player.movePosition = new Vector3(currentVelocity.x, Player.movePosition.y, currentVelocity.z);

        // 입력이 거의 없으면 Idle 상태로 전환
        if (input.magnitude < 0.01f && currentVelocity.magnitude < 0.01f) {
            Player.ChangeState(new PlayerIdleState());
        }
        
        // 입력에 따라 다른 상태로 전환
        HandleStateTransitions();
    }

    // 상태 전환 처리
    private void HandleStateTransitions() {
        // Shift 더블탭으로 구르기
        if (PlayerInput.ShiftTap) {
            if (Time.time - lastShiftTapTime < doubleTapThreshold) {
                Player.ChangeState(new PlayerRollState());
                lastShiftTapTime = -1f;
                return;
            }
            lastShiftTapTime = Time.time;
        }

        // 점프 입력 처리 (지면에 있을 때만)
        if (PlayerInput.JumpPressed && Player.controller.isGrounded) {
            animator.SetFloat(xMoveAnim, 0);
            animator.SetFloat(zMoveAnim, 0);
            Player.ChangeState(new PlayerJumpState());
            return;
        }

        // 공격 입력 처리 (무기를 장비한 상태에서만)
        if (PlayerInput.AttackPressed && Player.equipped) {
            animator.SetFloat(xMoveAnim, 0);
            animator.SetFloat(zMoveAnim, 0);
            Player.ChangeState(new PlayerAttackState());
            return;
        }

        // 무기 장착/해제 입력 처리
        if (PlayerInput.EquipPressed) {
            Player.WeaponEquip();
        }
    }

    // 애니메이션 파라미터 업데이트
    private void UpdateAnimation(Vector2 input, bool isRunning) {
        float targetAnimSpeed = isRunning ? 1f : 0.5f; // 달리기일 경우 1, 걷기일 경우 0.5
        speedLerp = Mathf.SmoothDamp(speedLerp, targetAnimSpeed, ref animSpeedVelocity, 0.1f); // 보간된 애니메이션 속도 계산

        float smoothTime = input.magnitude > 0.01f ? 0.1f : 0.13f; // 멈출 때 더 빠르게 감속
        animX = Mathf.SmoothDamp(animX, input.x * speedLerp, ref animXVelocity, smoothTime); // X축 애니메이션 값 보간
        animZ = Mathf.SmoothDamp(animZ, input.y * speedLerp, ref animZVelocity, smoothTime); // Z축 애니메이션 값 보간

        animator.SetFloat(xMoveAnim, animX); // 애니메이터에 값 적용
        animator.SetFloat(zMoveAnim, animZ);
    }

    // 입력 벡터를 카메라 방향 기준으로 변환
    private Vector3 GetMoveDirection(Vector2 input) {
        Vector3 forward = Player.cameraTransform.forward;
        Vector3 right = Player.cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return (right * input.x + forward * input.y).normalized; // 최종 이동 방향 반환
    }

    // 상태 종료 시 초기화
    public override void Exit() {
        /*animX = 0f;
        animZ = 0f;
        speedLerp = 0f;
        currentVelocity = Vector3.zero; // 멈출 때 완전히 정지시킴
        moveVelocity = Vector3.zero;*/
    }
}
