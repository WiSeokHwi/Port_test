using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    private Animator animator;
    
    private EnemyState currentState { get; set; }
    [SerializeField] private Vector3 target;
    public Vector3 Target => target;
    
    private Vector3 d1;       // 중간 회전 지점 (턴포인트)
    
    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    [SerializeField] private Transform homePosition;
    public Vector3 HomePosition => homePosition.position;

    public EnemySensor enenmySensor;
    
    public Transform[] waypoints;

    public float detectionRadius = 6f;
    public float checkRadius = 8f;
    public float checkAngle = 30f;
    public float checkOffsetAmount = 0.5f;
    public float cloakingCheckAmount = 3f;
    public float walkSpeed = 1.2f;
    public float runSpeed = 2.5f;
    
    // 현재 '회전 중' 상태인지 표시
    public bool isTurnRound;
    private bool hasSetTurnRoundDes = false;
    
    public float turnRadius = 4; // 회전 반경
    public int turnDegreeStepValue = 120; // 회전 각도 (기본 회전 커브의 각도)
    public int turnAngleThreshold = 90; // 회전 시작 각도 임계값 (이보다 크면 회전 시작)


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enenmySensor = GetComponentInChildren<EnemySensor>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        currentState = new EnemyIdleState(this);
        currentState.Enter();
    }

    void Update()
    {
        // 현재 방향과 목적지 사이의 각도를 확인
        Vector3 targetDir = Agent.destination - transform.position;
        
        // 목적지가 정면 기준으로 너무 뒤에 있다면 회전 시작
        if (Vector3.Angle(targetDir, transform.forward) > turnAngleThreshold && isTurnRound == false)
        {
            target = Agent.destination; // 진짜 목적지를 저장
            isTurnRound = true; // 회전 시작 상태로 변경
            Agent.autoBraking = false; // 자동 브레이크 꺼서 부드럽게 회전
        }
        
        if (isTurnRound)
        {
            // 첫 번째 회전 지점을 아직 설정하지 않았다면
            if (hasSetTurnRoundDes == false)
            {
                d1 = FindTurnPoint(Target); // 회전용 중간 목적지를 계산
                hasSetTurnRoundDes = true;
                Agent.SetDestination(d1); // 중간 지점으로 이동
            }

            // 현재 위치가 중간 지점에 가까워졌다면 다음 회전 지점을 다시 설정
            if (Vector3.Distance(transform.position, d1) <= Agent.stoppingDistance * 1.2f)
            {
                if (Vector3.Angle(Target - transform.position, transform.forward) > turnAngleThreshold)
                {
                    d1 = FindTurnPoint(Target); // 다음 턴포인트 계산
                    Agent.SetDestination(d1); // 다음 지점으로 이동
                }
            }

            // 목적지와의 각도가 임계값보다 작으면 회전 종료, 원래 목적지로 복귀
            if (Vector3.Angle(Target - transform.position, transform.forward) < turnAngleThreshold)
            {
                Agent.SetDestination(Target); // 원래 목적지로 설정
                hasSetTurnRoundDes = false;
                isTurnRound = false;
                Agent.autoBraking = true; // 다시 자동 브레이크 활성화
            }
        }
        
        float speedPercent = 0f;
        
        if (Agent.velocity.magnitude <= walkSpeed)
        {
            // Walk 구간: 0 ~ 0.5로 매핑
            speedPercent = Mathf.InverseLerp(0f, walkSpeed, Agent.velocity.magnitude) * 0.5f;
        }
        else
        {
            // Run 구간: 0.5 ~ 1.0로 매핑
            speedPercent = 0.5f + Mathf.InverseLerp(walkSpeed, runSpeed, Agent.velocity.magnitude) * 0.5f;
        }

        animator.SetFloat("MoveSpeed", speedPercent, 0.1f, Time.deltaTime);
        currentState.Update();
        Debug.Log("적 상태 : " + currentState);
    }

    void FixedUpdate()
    {   
        currentState.PhysicsUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        
        currentState.Enter();
    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
    }
    
    // 회전용 지점 계산 함수
    Vector3 FindTurnPoint(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        // 방향에 따라 왼쪽 회전할지 오른쪽 회전할지 결정 (Y축 기준으로 판단)
        var cross = Vector3.Cross(transform.forward, direction);

        Vector3 targetPos;
        NavMeshHit navMeshHit;

        if (cross.y < 0) // 목표가 왼쪽에 있을 경우
        {
            // 회전 곡선을 계산하여 왼쪽에 있는 턴포인트 계산
            targetPos = transform.position - transform.right * turnRadius
                                           - transform.right * (turnRadius * Mathf.Cos((180 - turnDegreeStepValue) * Mathf.Deg2Rad))
                        + transform.forward * (turnRadius * Mathf.Sin((180 - turnDegreeStepValue) * Mathf.Deg2Rad));
        }
        else // 목표가 오른쪽에 있을 경우
        {
            // 오른쪽 회전 곡선 계산
            targetPos = transform.position + transform.right * turnRadius
                                           + transform.right * (turnRadius * Mathf.Cos(turnDegreeStepValue * Mathf.Deg2Rad))
                                           + transform.forward * (turnRadius * Mathf.Sin(turnDegreeStepValue * Mathf.Deg2Rad));
        }

        // NavMesh에서 유효한 위치인지 샘플링 (2미터 범위)
        if (NavMesh.SamplePosition(targetPos, out navMeshHit, 2f, -1))
        {
            targetPos = navMeshHit.position; // 유효한 점이면 그 위치 사용
        }
        else
        {
            targetPos = this.target; // 없으면 그냥 원래 목적지로 이동
        }

        return targetPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, cloakingCheckAmount);
    }
}

