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

    
    private float time;

    public override void Enter()
    {
        base.Enter();
        agent.speed = enemy.runSpeed; // 뛰는 속도로 설정
        // EnemyController에서 설정 값 가져오기
        detectionRadius = enemy.detectionRadius;
        // EnemyController에 이 변수들이 있어야 합니다.
        // 예: public float attackRange = 2f;
        // 예: public float attackAngle = 90f; // 예시 값 (총 90도 시야각)
        attackRange = enemy.attackRange;
        attackAngle = enemy.attackAngle;

        time = 2f; // 마지막 감지 후 추격 유지 시간

        // Enter 시점에서 현재 Agent의 목적지를 detectedPosition으로 설정
        agent.SetDestination(enemy.Target.transform.position);
    }

    public override void Update()
    {
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
        
        foreach (GameObject target in enemy.targets)
        {
            // 타겟과 자신사이의 거리를 구함
            float distance = Vector3.Distance(target.transform.position, enemy.transform.position);

            if (distance <= enemy.detectionRadius) //체크거리보다 distance가 작다면(영역내로 들어왔다면)
            {
                RaycastHit hit;
                
                if (!Physics.Raycast(enemy.transform.position, target.transform.position, out hit, enemy.checkRadius, // 나와 플레이어 사이에 장애물이 없다면
                        obstacleLayer))
                {
                    Debug.DrawRay(enemy.transform.position, target.transform.position * enemy.checkRadius, Color.red);
                    enemy.SetTarget(target);
                    agent.SetDestination(enemy.Target.transform.position);
                }
            }
            else
            {
                
            }
        }
        

        
        


        // === 추적 로직 (공격 가능 대상이 없을 때 실행) ===
        bool playerDetectedBroadly = false; // 넓은 범위 감지 여부 플래그

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