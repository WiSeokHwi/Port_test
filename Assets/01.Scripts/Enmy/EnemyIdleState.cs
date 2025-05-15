
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy) : base(enemy){ }
    private int pathIndex;
    private Vector3 getTarget;
    private float cloakingCheckAmount;
    private float timer;
    private LayerMask layerMask = LayerMask.NameToLayer("Obstacle");
    public override void Enter()
    {
        base.Enter(); 
        agent.isStopped = false;
        getTarget = Vector3.zero;
        enemy.SetTarget(enemy.HomePosition);
        agent.SetDestination(enemy.Target);
        agent.speed = enemy.walkSpeed;
        cloakingCheckAmount = enemy.cloakingCheckAmount;

    }

    public override void Update()
    {
        foreach (GameObject target in targets)
        {
            // 타겟과 자신사이의 거리를 구함
            float distance = Vector3.Distance(target.transform.position, enemy.transform.position);

            if (distance <= enemy.checkRadius) //체크거리보다 distance가 작다면(영역내로 들어왔다면)
            {
                // 타겟 방향 구하기
                Vector3 dirToTarget = target.transform.position - enemy.transform.position;
                dirToTarget.Normalize(); // 방향만 가져오기
                
                // 자신이 바라보는 방향
                Vector3 foward = enemy.transform.forward;
                
                // 앵글 계산
                float angle = Vector3.Angle(foward, dirToTarget);
                if (angle <= enemy.checkAngle * 0.5f)
                {
                    // 오프셋을 적용한 방향으로 Raycast
                    RaycastHit hit;

                    // 방향 벡터에 오프셋을 동시에 적용한 방향
                    Vector3 leftOffsetDirection = Quaternion.Euler(0, -enemy.checkOffsetAmount, 0) * dirToTarget;
                    Vector3 rightOffsetDirection = Quaternion.Euler(0, enemy.checkOffsetAmount, 0) * dirToTarget;

                    // 좌측과 우측 오프셋 범위 내에서 장애물이 있는지 한 번에 체크
                    if (!Physics.Raycast(enemy.transform.position, leftOffsetDirection, out hit, enemy.checkRadius, layerMask) &&
                        !Physics.Raycast(enemy.transform.position, rightOffsetDirection, out hit, enemy.checkRadius, layerMask))
                    {
                        
                    }
                }
            }
        }
        
        // 도착지점에 도달했을 때
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            float waitTime = 2f;
            
            timer += Time.deltaTime;
            
            if (timer >= waitTime)
            {
                WanderPosLoop();
            }
        }
    }

    void ChaingeCheckState()
    {
        
    }
    void WanderPosLoop()
    {

            // 웨이포인트 인덱스 업데이트
            if (pathIndex < enemy.waypoints.Length - 1)
            {
                pathIndex++;
            }
            else
            {
                pathIndex = 0;
            }

            // 새로운 타겟 설정
            enemy.SetTarget(enemy.waypoints[pathIndex].transform.position);
            agent.SetDestination(enemy.Target);
            timer = 0f;
    }
}
