using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController enemy) : base(enemy)
    {
    }
    
    private LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
    

    // attackRange, attackAngle 변수는 EnemyController에서 가져와야 함
    // EnemyController에 attackRange, attackAngle 프로퍼티 또는 필드 필요
    private float attackRange;
    private float attackAngle;
    
    private Vector3 lastTargetPosition;
    private Vector3 predictedPosition;
    private Vector3 targetVelocity;
    private bool isDetected;

    
    private float time;

    public override void Enter()
    {
        agent.speed = enemy.runSpeed; // 뛰는 속도로 설정
        attackRange = enemy.attackRange;
        attackAngle = enemy.attackAngle;
        isDetected = false; // 감지상태 초기화

        if (enemy.Target != null)
        {
            predictedPosition = enemy.Target.transform.position;
        }
        
        
        time = 2f; // 마지막 감지 후 추격 유지 시간

        if (enemy.Target != null)
        {
            lastTargetPosition = enemy.Target.transform.position;
            agent.SetDestination(lastTargetPosition);
        }
    }

    public override void Update()
    {
       
        PredictedChase();
    }

    

    void PredictedChase()
    {
        foreach (GameObject target in enemy.targets)
        {
            
            // 타겟과 자신사이의 거리를 구함
            float distance = Vector3.Distance(target.transform.position, enemy.transform.position);
            Vector3 direction = (target.transform.position - enemy.transform.position).normalized;

            if (distance <= enemy.attackRange)
            {
                float angleToTarget = Vector3.Angle(enemy.transform.forward, direction);
                
                if (angleToTarget <= enemy.attackAngle * 0.5f)
                {
                    enemy.SetTarget(target);
                    enemy.ChangeState(new EnemyAttackState(enemy));
                }
            }
            
            if (distance <= enemy.detectionRadius) //체크거리보다 distance가 작다면(영역내로 들어왔다면)
            {
                RaycastHit hit;
                
                if (!Physics.Raycast(enemy.transform.position, direction, out hit, enemy.detectionRadius, obstacleLayer))
                {
                    isDetected = true;
                    enemy.SetTarget(target);
                    time = 2f;
                    
                    // 속도 예측 로직
                    Vector3 currentTargetPosition = target.transform.position; // 타겟의 현재 위치 저장
                    
                    // 현재 위치 - 마지막 위치 = 이동량 을 Time.deltaTime 로 나눠 프레임당 변화율
                    targetVelocity = (currentTargetPosition - lastTargetPosition) / Time.deltaTime; 
                    
                    // lastTargetPosition 을 현재 위치로 변경 
                    lastTargetPosition = currentTargetPosition; 

                    float predictionTime = 0.5f; // 예측 시간 (0.5초 후)
                    
                    // 예측 이동값 현재 위치 + 프래임당 변화량 * 예측할 시간
                    predictedPosition = currentTargetPosition + targetVelocity * predictionTime; 

                    // 타겟의 현재 위치에서 예측위치로 향하는 방향
                    Vector3 predictedDir = (predictedPosition - currentTargetPosition).normalized;
                    
                    float predictedDistance = Vector3.Distance(currentTargetPosition, predictedPosition);
                    
                    if (!Physics.Raycast(currentTargetPosition, predictedDir, predictedDistance, obstacleLayer))
                    {
                        agent.SetDestination(predictedPosition);
                        Debug.DrawRay(currentTargetPosition, predictedDir * predictedDistance, Color.magenta);
                    }
                    else
                    {
                        // 예측 경로에 장애물이 있다면 그냥 현재 위치로 이동
                        agent.SetDestination(currentTargetPosition);
                        
                    }
                    
                }
                else
                {
                    isDetected = false;
                }

                if (!isDetected)
                {
                    MissingTarget();
                }
                
            }
            else
            {
                MissingTarget();
            }

        }
    }

    void MissingTarget()
    {
        if (enemy.Target)
        {
                        
            enemy.SetTarget(null);
            agent.SetDestination(predictedPosition);
        }

        // detectedPosition으로 이동 중이거나 이미 도달한 상태
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // 마지막 감지 위치에 도달함
            time -= Time.deltaTime; // 타이머 감소
            if (time <= 0f)
            {
                // 일정 시간 이상 플레이어를 다시 감지 못함 -> 추적 포기
                enemy.ChangeState(new EnemyIdleState(enemy));
            }
        }
        else
        {
            time = 2f;
        }
    }
    
}