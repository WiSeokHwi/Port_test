using UnityEngine;

public class EnemyCheckState : EnemyState
{
    public EnemyCheckState(EnemyController enemy) : base(enemy) {}
    private float timer = 0;
    private float waitTime = 1.5f;
    private LayerMask obstacleLayerMask = LayerMask.GetMask("Obstacle");
    
    public override void Enter()
    {
        base.Enter();
        agent.SetDestination(enemy.Target.transform.position);
        agent.speed = enemy.walkSpeed;
    }

    public override void Update()
    {
        ChangeCheckState();
        ChangeChasingState();
        // agent가 도착했는지 체크
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true; // 도착했으면 멈추기
            timer += Time.deltaTime;
    
            if (timer >= waitTime)
            {
                enemy.ChangeState(new EnemyIdleState(enemy));
            }
        }
        
        
        
    }
    public override void Exit()
    {
        base.Exit();
        timer = 0;
    }

    public void ChangeCheckState()
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
                    
                    RaycastHit hit;

                    if (!Physics.Raycast(enemy.transform.position, target.transform.position, out hit, enemy.checkRadius,
                            obstacleLayerMask))
                    {
                        Debug.DrawRay(enemy.transform.position, target.transform.position * enemy.checkRadius, Color.red);
                        enemy.SetTarget(target);
                        agent.SetDestination(enemy.Target.transform.position);
                    }

                }
            }
        }
    }

    public void ChangeChasingState()
    {
        foreach (GameObject target in enemy.targets)
        {
            // 타겟과 자신사이의 거리를 구함
            float distance = Vector3.Distance(target.transform.position, enemy.transform.position);

            if (distance <= enemy.detectionRadius) //체크거리보다 distance가 작다면(영역내로 들어왔다면)
            {
                enemy.SetTarget(target);
                enemy.ChangeState(new EnemyChaseState(enemy));
            }
        }
    }
}
