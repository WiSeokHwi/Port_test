using UnityEngine;

public class EnemyCheckState : EnemyState
{
    public EnemyCheckState(EnemyController enemy) : base(enemy) {}
    private float timer = 0;
    private float waitTime = 1.5f;
    private LayerMask detectionLayer = LayerMask.GetMask("Player");
    private LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
    private float detectionRadius;
    private Vector3 getTarget;

    public override void Enter()
    {
        base.Enter();
        agent.SetDestination(enemy.Target);
        agent.speed = enemy.walkSpeed;
        detectionRadius = enemy.detectionRadius;
    }

    public override void Update()
    {
        getTarget = enemy.enenmySensor.DetectEnemies();
        Debug.Log("타겟 : " + getTarget);
        if (getTarget != Vector3.zero)
        {
            enemy.SetTarget(getTarget);
            agent.SetDestination(enemy.Target);
        }
        
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
         Collider[] detectedColliders = Physics.OverlapSphere(enemy.transform.position, detectionRadius, detectionLayer);

         foreach (Collider col in detectedColliders)
         {
             if (!Physics.Linecast(enemy.transform.position, col.transform.position, out RaycastHit hit, obstacleLayer))
             {
                 enemy.ChangeState(new EnemyChaseState(enemy));
             }
         }
        
    }

    

    public override void Exit()
    {
        base.Exit();
        timer = 0;
    }

    
}
