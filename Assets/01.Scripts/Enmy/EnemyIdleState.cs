using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy) : base(enemy){ }
    private int pathIndex;
    private float timer;
    public override void Enter()
    {
        base.Enter(); 
        agent.isStopped = false;
        enemy.SetTarget(enemy.HomePosition);
        agent.SetDestination(enemy.Target.position);
        
    }

    public override void Update()
    {
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
                enemy.SetTarget(enemy.waypoints[pathIndex]);
                agent.SetDestination(enemy.Target.position);
            }
        }
    }


}
