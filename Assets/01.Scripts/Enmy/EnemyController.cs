using UnityEngine;
using UnityEngine.AI; // NavMeshAgent 사용을 위해 필수

public class EnemyController : MonoBehaviour
{
    private Animator animator; // 애니메이션 제어를 위한 Animator 컴포넌트

    // 현재 AI의 상태 (예: Idle, Patrol, Chase 등). 상태 패턴 사용
    private EnemyState currentState { get; set; } 
    
    // 최종 목표 지점 (회전 로직 중 실제 목표 지점을 저장하기 위해 사용)
    [SerializeField] private Vector3 target; 
    public Vector3 Target => target; // 외부에서 읽기 전용으로 접근 가능하도록 프로퍼티 설정

    // 회전 시 사용할 중간 경유 지점 (턴 포인트)
    private Vector3 d1;       

    // NavMeshAgent 컴포넌트. 길찾기 및 이동 제어 담당
    [SerializeField] private NavMeshAgent agent; 
    public NavMeshAgent Agent => agent; // 외부 접근용 프로퍼티

    // 몬스터의 초기 위치 또는 복귀 지점
    [SerializeField] private Transform homePosition; 
    public Vector3 HomePosition => homePosition.position; // 외부 접근용 프로퍼티

    // 적 감지 로직을 담당하는 센서 컴포넌트 (별도 스크립트로 추정)
    public EnemySensor enenmySensor; 
    
    // 순찰 경로 지점들 (필요시 사용)
    public Transform[] waypoints; 
    
    // 새로 만든 시각화 관리 스크립트 참조
    [SerializeField] private AttackConeVisualizer attackConeVisualizer;

    // === AI 설정값들 (Inspector에서 조절 가능) ===
    public float detectionRadius = 6f; // 적 감지 반경
    public float checkRadius = 8f;     // (사용처 불분명, EnemySensor에서 사용할 가능성)
    public float checkAngle = 30f;     // (사용처 불분명, EnemySensor에서 사용할 가능성)
    public float checkOffsetAmount = 0.5f; // (사용처 불분명, EnemySensor에서 사용할 가능성)
    public float cloakingCheckAmount = 3f; // (사용처 불분명, 은신 감지 등에 사용할 가능성)
    public float attackRange = 1.5f;
    public float attackAngle = 80f;
    
    
    public float walkSpeed = 1.2f;     // 걷기 속도 (애니메이션 제어용)
    public float runSpeed = 2.5f;      // 뛰기 속도 (애니메이션 및 Agent 속도 설정용)
    
    // === 회전 로직 관련 변수 ===
    // 현재 '회전 중' 상태인지 표시하는 플래그
    public bool isTurnRound; 
    
    public float turnRadius = 1; // 회전 반경 (클수록 크게 돎)
    public int turnDegreeStepValue = 60; // 1회전 시 꺾는 각도 (클수록 급격하게 돎)
    public int turnAngleThreshold = 90; // 회전을 시작할 각도 임계값 (현재 방향과 목표 방향 사이 각도가 이 값보다 크면 회전 시작)


    // 컴포넌트 참조 설정 (게임 오브젝트 활성화 시 1회 호출)
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
        enenmySensor = GetComponentInChildren<EnemySensor>(); // 자식 오브젝트에서 EnemySensor 컴포넌트 가져오기
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        
        if (attackConeVisualizer == null)
        {
            attackConeVisualizer = GetComponentInChildren<AttackConeVisualizer>();
            if (attackConeVisualizer == null)
            {
                Debug.LogError("AttackConeVisualizer component not found! Assign it in the Inspector or ensure it's a child.");
                enabled = false; // 시각화 없으면 스크립트 비활성화 등 에러 처리
                return;
            }
        }
    }
    
    // 초기 상태 설정 (첫 프레임 업데이트 전 1회 호출)
    void Start()
    {
        // 초기 상태를 EnemyIdleState로 설정하고 진입 로직 실행
        currentState = new EnemyIdleState(this); 
        currentState.Enter();
        // Start 시점에서 시각화 스크립트의 초기 상태 설정 (완전 투명)
        if (attackConeVisualizer != null)
        {
            attackConeVisualizer.StartFadeOut(); // 처음에 완전히 숨김
        }
    }

    // 매 프레임 호출 (주요 로직 업데이트)
    void Update()
    {
        if (attackConeVisualizer != null)
        {
            attackConeVisualizer.EnemyPosition = transform.position;
            attackConeVisualizer.EnemyForward = transform.forward;
            attackConeVisualizer.AttackRange = attackRange;
            attackConeVisualizer.AttackAngle = attackAngle;
            // AttackConeVisualizer의 Update 함수가 자동으로 실행되면서 메쉬 업데이트 및 셰이더 변수 전달
        }
        
        TurnRound();

        // --- 애니메이션 속도 제어 로직 ---
        float speedPercent = 0f; // 애니메이터에 전달할 속도 비율 (0:정지, 0.5:걷기 최대, 1:뛰기 최대)
        
        // 현재 NavMeshAgent의 속도(Agent.velocity.magnitude)가 걷기 속도(walkSpeed) 이하라면
        if (Agent.velocity.magnitude <= walkSpeed)
        {
            // 현재 속도를 0 ~ walkSpeed 범위에서 0 ~ 0.5 범위로 변환 (정규화 후 0.5 곱하기)
            // Mathf.InverseLerp(min, max, value): value가 min일 때 0, max일 때 1을 반환
            speedPercent = Mathf.InverseLerp(0f, walkSpeed, Agent.velocity.magnitude) * 0.5f;
        }
        else // 현재 속도가 걷기 속도보다 빠르다면 (뛰는 중)
        {
            // 현재 속도를 walkSpeed ~ runSpeed 범위에서 0.5 ~ 1.0 범위로 변환
            // (walkSpeed일 때 0, runSpeed일 때 1로 정규화한 값에 0.5를 더함)
            speedPercent = 0.5f + Mathf.InverseLerp(walkSpeed, runSpeed, Agent.velocity.magnitude) * 0.5f;
        }

        // Animator의 "MoveSpeed" 파라미터 값을 계산된 speedPercent로 설정
        // 0.1f: 값 변경 시 부드럽게 전환되는 시간 (Damp Time)
        // Time.deltaTime: 프레임 간 시간 간격 (프레임 속도에 관계없이 일정한 속도로 값 변경)
        animator.SetFloat("MoveSpeed", speedPercent, 0.1f, Time.deltaTime);
        
        // --- 애니메이션 속도 제어 로직 끝 ---

        // 현재 상태(currentState)의 Update 로직 실행
        currentState.Update();
        
        // 디버깅용 로그: 현재 어떤 상태인지 출력
        Debug.Log("적 상태 : " + currentState);
    }

    // 고정된 시간 간격으로 호출 (주로 물리 관련 로직 처리)
    void FixedUpdate()
    {   
        // 현재 상태의 PhysicsUpdate 로직 실행 (필요하다면)
        currentState.PhysicsUpdate();
    }

    // AI 상태 변경 함수
    public void ChangeState(EnemyState newState)
    {
        // 현재 상태가 존재하면 Exit 로직 호출 (상태 종료 시 정리 작업)
        currentState?.Exit(); 
        // 새로운 상태로 변경
        currentState = newState; 
        // 새로운 상태의 Enter 로직 호출 (상태 시작 시 초기화 작업)
        currentState.Enter(); 
    }

    // 외부에서 목표 지점을 설정하는 함수 (현재 코드에서는 직접 사용되지 않는 듯 보임)
    // 회전 로직은 Agent.destination을 기준으로 동작하고, 회전 시작 시점에 target 변수에 백업함.
    // 이 함수가 필요한 경우는 상태 변경 시 목표를 명시적으로 지정할 때일 수 있음.
    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
    }

    private void TurnRound()
    {
        // --- 회전 로직 시작 ---
    
        // 목표 지점(Agent.destination) 방향 벡터 계산 (목표지점 - 현재위치)
        Vector3 targetDir = Agent.destination - transform.position;
    
        // 최종 목표 방향과 현재 바라보는 방향 사이의 각도
        float angleToTarget = Vector3.Angle(targetDir, transform.forward);

        // 목표 지점이 현재 바라보는 방향 기준 뒤쪽에 있는지 확인 (각도가 크고)
        // 현재 회전 중(isTurnRound)이 아니라면 회전 시작
        // 회전 중이 아닐 때만 isTurnRound 시작 조건을 체크
        if (isTurnRound == false && angleToTarget > turnAngleThreshold)
        {
            // 회전 시작 시점에만 최종 목적지를 'target' 변수에 백업
            // (Agent.destination은 중간 경유지로 계속 바뀔 것이므로)
            target = Agent.destination; 
            isTurnRound = true; // '회전 중' 상태로 변경
            // NavMeshAgent가 목적지 근처에서 자동으로 감속 및 정지하는 기능 비활성화
            Agent.autoBraking = false; 
            // 회전 시작 시 첫 중간 지점은 바로 계산하여 설정
            d1 = FindTurnPoint(Target); // 첫 중간 경유지 계산
            Agent.SetDestination(d1);   // 첫 중간 경유지로 이동 시작
        }
    
        // '회전 중' 상태일 때 실행되는 로직
        if (isTurnRound)
        {
            // **핵심 변경**: 회전 중일 때는 매 프레임마다 현재 위치에서 계산된 다음 중간 경유지(d1)를 목표로 설정
            // 이렇게 하면 에이전트가 지속적으로 회전 경로 상의 다음 지점을 따라가게 됩니다.
            d1 = FindTurnPoint(Target); // 현재 위치를 기준으로 회전 경로상의 다음 목표 지점 계산
            Agent.SetDestination(d1); // 계산된 지점으로 Agent의 목표 갱신

            // 현재 방향과 최종 목적지(Target) 사이의 각도가 임계값(turnAngleThreshold)보다 작아졌다면
            // (즉, 충분히 목표 방향으로 몸을 돌렸다면) 회전 종료
            // 주의: 각도 계산 시 Target까지의 방향 벡터를 사용해야 합니다.
            if (Vector3.Angle(Target - transform.position, transform.forward) < turnAngleThreshold)
            {
                Agent.SetDestination(Target); // Agent의 목표를 원래의 최종 목적지(Target)로 설정
                isTurnRound = false; // '회전 중' 상태 종료
                Agent.autoBraking = true; // 목적지 자동 감속/정지 기능 다시 활성화
                
            }
            // else // 디버깅: 회전 중 각도 확인
            // {
            //     Debug.Log("턴 중 - 현재 목표 각도: " + Vector3.Angle(Target - transform.position, transform.forward));
            // }
        }
    }
    
    // 회전을 위한 중간 경유 지점(Turn Point)을 계산하는 함수
    Vector3 FindTurnPoint(Vector3 target)
    {
        // 현재 위치에서 최종 목표 지점(target)까지의 방향 벡터
        Vector3 direction = target - transform.position;

        // 현재 바라보는 방향(transform.forward)과 목표 방향(direction)의 외적(Cross Product) 계산
        // 외적 결과의 Y 값 부호로 목표 지점이 현재 방향 기준 왼쪽에 있는지 오른쪽에 있는지 판단
        // (Unity 좌표계 기준: Y축이 위쪽일 때)
        //   - cross.y < 0 : 목표가 왼쪽에 있음 (왼쪽으로 회전 필요)
        //   - cross.y > 0 : 목표가 오른쪽에 있음 (오른쪽으로 회전 필요)
        //   - cross.y == 0: 목표가 정확히 앞 또는 뒤에 있음 (이 경우는 보통 Angle 체크에서 걸러지거나, 오른쪽으로 처리됨)
        var cross = Vector3.Cross(transform.forward, direction);

        Vector3 targetPos; // 계산될 중간 경유지 위치
        NavMeshHit navMeshHit; // NavMesh 위 유효한 위치 정보를 담을 변수

        if (cross.y < 0) // 목표가 왼쪽에 있을 경우 (왼쪽으로 돌아야 함)
        {
            // 원호 상의 점을 계산하여 왼쪽 중간 경유지 위치 계산
            // 현재 위치(transform.position)에서
            // 왼쪽(-transform.right)으로 turnRadius 만큼 이동하고,
            // turnDegreeStepValue 각도만큼 회전한 위치를 삼각함수(Cos, Sin)를 이용해 계산하여 더함.
            // Deg2Rad: 각도를 라디안으로 변환
            // (180 - turnDegreeStepValue) 를 사용하는 것은 왼쪽으로 도는 각도를 계산하기 위함으로 보임.
            targetPos = transform.position - transform.right * turnRadius // 기본 원점 이동 (왼쪽)
                                           - transform.right * (turnRadius * Mathf.Cos((180 - turnDegreeStepValue) * Mathf.Deg2Rad)) // 원호 X좌표
                                           + transform.forward * (turnRadius * Mathf.Sin((180 - turnDegreeStepValue) * Mathf.Deg2Rad)); // 원호 Z좌표
        }
        else // 목표가 오른쪽에 있거나 정면에 있을 경우 (오른쪽으로 돌아야 함)
        {
            // 위와 유사하게, 오른쪽 중간 경유지 위치 계산
            targetPos = transform.position + transform.right * turnRadius // 기본 원점 이동 (오른쪽)
                                           + transform.right * (turnRadius * Mathf.Cos(turnDegreeStepValue * Mathf.Deg2Rad)) // 원호 X좌표
                                           + transform.forward * (turnRadius * Mathf.Sin(turnDegreeStepValue * Mathf.Deg2Rad)); // 원호 Z좌표
        }

        // 계산된 targetPos 위치가 NavMesh 위에 있는지 확인 (SamplePosition)
        // targetPos 주변 2.0f 반경 내에서 가장 가까운 NavMesh 위의 점을 찾음 (-1은 모든 NavMesh 영역을 의미)
        if (NavMesh.SamplePosition(targetPos, out navMeshHit, 2f, NavMesh.AllAreas)) // NavMesh.AllAreas 대신 -1 사용 가능
        {
            // NavMesh 위 유효한 위치를 찾았다면, 그 위치(navMeshHit.position)를 사용
            targetPos = navMeshHit.position; 
        }
        else
        {
            // 만약 계산된 위치 주변에 유효한 NavMesh가 없다면 (예: 벽 속, 허공)
            // 안전하게 원래의 최종 목적지(this.target)를 중간 경유지로 사용 (회전이 부자연스러워질 수 있음)
            // TODO: 더 나은 대안 고려 가능 (예: 현재 위치에서 약간 앞쪽 지점 등)
            targetPos = this.target; 
        }

        // 최종적으로 계산되고 검증된 중간 경유지 위치 반환
        return targetPos;
    }
    public void StartAttackConeVizFadeIn()
    {
        if (attackConeVisualizer != null)
        {
            attackConeVisualizer.StartFadeIn();
        }
    }

    public void StartAttackConeVizFadeOut()
    {
        if (attackConeVisualizer != null)
        {
            attackConeVisualizer.StartFadeOut();
        }
    }

    // Scene 뷰에서 Gizmo를 그리는 함수 (디버깅 및 시각화 목적)
    private void OnDrawGizmos()
    {
        // 감지 반경(detectionRadius)을 Cyan 색상 원으로 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // 은신 감지 반경(cloakingCheckAmount)을 Magenta 색상 원으로 표시
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, cloakingCheckAmount);
        
        // === 공격 범위 및 각도 시각화 ===
        if (attackRange > 0) // 공격 반경이 0보다 클 때만 그림
        {
            // 공격 반경을 빨간색 원으로 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // 공격 각도를 부채꼴 형태로 표시 (라인으로 경계 그리기)
            if (attackAngle < 180f) // 각도가 180도 이상이면 전체 원이므로 각도 표시는 생략
            {
                Vector3 forward = transform.forward; // 적의 현재 전방 방향
                Vector3 position = transform.position; // 적의 위치
                float halfAttackAngle = attackAngle / 2f; // 전방에서 좌우로 벌어지는 각도

                // Quaternion.AngleAxis를 사용하여 전방 벡터를 Y축(Up) 기준으로 좌우로 회전시켜 경계 방향을 얻습니다.
                Quaternion leftRotation = Quaternion.AngleAxis(-halfAttackAngle, transform.up); // 왼쪽 경계 방향 회전
                Vector3 leftDirection = leftRotation * forward; // 회전된 왼쪽 경계 방향 벡터

                Quaternion rightRotation = Quaternion.AngleAxis(halfAttackAngle, transform.up); // 오른쪽 경계 방향 회전
                Vector3 rightDirection = rightRotation * forward; // 회전된 오른쪽 경계 방향 벡터

                // 적 위치에서 공격 반경 끝의 경계 방향까지 라인을 그립니다.
                Gizmos.DrawLine(position, position + leftDirection * attackRange);
                Gizmos.DrawLine(position, position + rightDirection * attackRange);

                // 선택적으로, 공격 반경 끝에서 경계 라인 사이의 호를 그릴 수 있습니다.
                // Gizmos.DrawWireArc 함수가 없으므로, 여러 개의 작은 라인을 이어붙이거나 생략합니다.
                // 일반적으로 경계 라인 두 개와 원만 그려도 충분합니다.
            }
            else if (attackAngle >= 180f)
            {
                // 180도 이상이면 거의 전 방향이므로, 각도 라인은 특별히 그리지 않습니다.
            }
        }
        // === 공격 범위 및 각도 시각화 끝 ===
    }
}