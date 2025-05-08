using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController enemy) : base(enemy)
    {
    }

    private LayerMask detectionLayer = LayerMask.GetMask("Player");
    private LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
    private float detectionRadius;

    // attackRange, attackAngle 변수는 EnemyController에서 가져와야 함
    // EnemyController에 attackRange, attackAngle 프로퍼티 또는 필드 필요
    private float attackRange;
    private float attackAngle;

    private Vector3 detectedPosition;
    private float time;

    public override void Enter()
    {
        base.Enter();
        agent.speed = enemy.runSpeed; // 뛰는 속도로 설정
        detectedPosition = enemy.Target;

        // EnemyController에서 설정 값 가져오기
        detectionRadius = enemy.detectionRadius;
        // EnemyController에 이 변수들이 있어야 합니다.
        // 예: public float attackRange = 2f;
        // 예: public float attackAngle = 90f; // 예시 값 (총 90도 시야각)
        attackRange = enemy.attackRange;
        attackAngle = enemy.attackAngle;

        time = 2f; // 마지막 감지 후 추격 유지 시간

        // Enter 시점에서 현재 Agent의 목적지를 detectedPosition으로 설정
        agent.SetDestination(detectedPosition);
    }

    public override void Update()
    {
        // 넓은 범위 감지 (추적 대상 확인용)
        Collider[] detectedColliders = Physics.OverlapSphere(enemy.transform.position, detectionRadius, detectionLayer);

        // 공격 범위 감지
        Collider[] attackColliders = Physics.OverlapSphere(enemy.transform.position, attackRange, detectionLayer);

        // === 공격 가능 대상 확인 및 상태 전환 로직 ===
        // 공격 범위 안에 있는 대상들을 순회하며
        foreach (Collider col in attackColliders)
        {
            // 대상의 위치
            Vector3 targetPosition = col.transform.position;

            // 적(enemy)의 현재 위치에서 대상까지의 방향 벡터
            Vector3 directionToTarget = targetPosition - enemy.transform.position;

            // 적(enemy)이 현재 바라보는 방향(transform.forward)과 대상까지의 방향 벡터 사이의 각도 계산
            float angleToTarget = Vector3.Angle(enemy.transform.forward, directionToTarget);

            // 계산된 각도가 attackAngle의 절반(attackAngle / 2)보다 작거나 같으면
            // (즉, 대상이 적의 attackAngle 시야 범위 안에 있다면)
            if (angleToTarget <= attackAngle / 2f)
            {
                // 추가: 만약 공격 전에 시야 차단 여부를 확인하고 싶다면 Linecast 사용
                // Linecast가 obstacleLayer에 부딪히지 않았다면 (즉, 시야가 확보된다면)
                if (!Physics.Linecast(enemy.transform.position, targetPosition, obstacleLayer))
                {
                    // 공격 상태로 전환 (EnemyAttackState는 별도로 구현되어 있어야 함)
                    // 예시: enemy.ChangeState(new EnemyAttackState(enemy)); 
                    
                    enemy.ChangeState(new EnemyAttackState(enemy,col));
                    return; // Update 함수를 즉시 종료하고 다음 프레임에 새로운 상태의 Update 실행
                }
            }
        }
        // === 공격 가능 대상 확인 로직 끝 ===


        // === 추적 로직 (공격 가능 대상이 없을 때 실행) ===
        bool playerDetectedBroadly = false; // 넓은 범위 감지 여부 플래그

        // 넓은 범위 감지 콜라이더 순회 (추적 대상의 시야 확보 확인)
        foreach (Collider col in detectedColliders)
        {
            // 대상의 위치
            Vector3 targetPosition = col.transform.position;
            
            // 적 위치에서 대상까지 시야 차단이 없는지 확인
            if (!Physics.Linecast(enemy.transform.position, targetPosition, out RaycastHit hit, obstacleLayer))
            {
                // 시야 확보됨: 플레이어 발견!
                playerDetectedBroadly = true; // 감지 플래그 설정
                time = 2f; // 마지막 감지 시간 초기화
                agent.SetDestination(col.gameObject.transform.position); // 플레이어 위치로 목적지 설정
                 // 마지막 감지 위치 갱신
                detectedPosition = targetPosition;
                // 이 루프는 여러 대상이 있을 수 있지만, 추적은 한 대상을 향해 하므로 여기서는 break 하지 않습니다.
                // 가장 가까운 대상 등 우선순위 로직을 추가할 수도 있습니다.
                break; // 첫 번째 시야 확보된 대상에게만 반응하도록 break
            }
        }

        // 넓은 범위에서 플레이어가 시야 확보되지 않았을 경우 (또는 detectedColliders가 비어있을 경우)
        if (!playerDetectedBroadly)
        {
            // 마지막으로 감지했던 위치로 이동 (Enter에서 이미 설정됨)
            agent.SetDestination(detectedPosition);
            // detectedPosition으로 이동 중이거나 이미 도달한 상태
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 마지막 감지 위치에 도달함
                time -= Time.deltaTime; // 타이머 감소
                if (time <= 0f)
                {
                    // 일정 시간 이상 플레이어를 다시 감지 못함 -> 추적 포기
                    Debug.Log("추적 대상 감지 실패 시간 초과. Idle 상태로 전환.");
                    enemy.ChangeState(new EnemyIdleState(enemy));
                }
            }
            // 만약 detectedPosition으로 가는 도중이라면 계속 그곳으로 이동합니다.
        }
        
    }
}