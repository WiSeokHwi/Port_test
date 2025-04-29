
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy) : base(enemy){ }
    private int pathIndex;
    private float timer;
    private Vector3 getTarget;
    private float cloakingCheckAmount;
    private LayerMask detectionLayer = LayerMask.GetMask("Player");
    public override void Enter()
    {
        base.Enter(); 
        agent.isStopped = false;
        enemy.SetTarget(enemy.HomePosition);
        agent.SetDestination(enemy.Target);
        agent.speed = enemy.walkSpeed;
        cloakingCheckAmount = enemy.cloakingCheckAmount;

    }

    public override void Update()
    {
        getTarget = Vector3.zero;
        getTarget = enemy.enenmySensor.DetectEnemies();
        
        // 도착지점에 도달했을 때
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;  // 매 프레임 경과 시간 더함

            // 타이머가 1초가 되면
            if (timer >= 1.5f)
            {
                // 타이머 초기화
                timer = 0f;

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
            }
        }
        Collider[] detectedColliders = Physics.OverlapSphere(enemy.transform.position, cloakingCheckAmount, detectionLayer);

        foreach (Collider col in detectedColliders)
        {
            if (!col.GetComponent<PlayerController>().isCloaking)
            {
                enemy.ChangeState(new EnemyChaseState(enemy));
            }
        }

        if (getTarget != Vector3.zero)
        {
            enemy.SetTarget(getTarget);
            enemy.ChangeState(new EnemyCheckState(enemy));
        }
    }


}
