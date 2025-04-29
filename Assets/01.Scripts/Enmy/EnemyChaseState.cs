using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController enemy) : base(enemy) {}
    
    private LayerMask detectionLayer = LayerMask.GetMask("Player");
    private LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
    private float detectionRadius;
    private Vector3 detectedPosition;
    private float time;

    public override void Enter()
    {
        base.Enter();
        agent.speed = 2.5f;
        detectionRadius = enemy.detectionRadius;
        time = 2f;
    }

    public override void Update()
    {
        
        Collider[] detectedColliders = Physics.OverlapSphere(enemy.transform.position, detectionRadius, detectionLayer);

        foreach (Collider col in detectedColliders)
        {
            
            
            if (!Physics.Linecast(enemy.transform.position, col.transform.position, out RaycastHit hit, obstacleLayer))
            {
                time = 2f;
                agent.SetDestination(col.transform.position);
                detectedPosition = col.transform.position;
            }
            else
            {
                agent.SetDestination(detectedPosition);
                

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    time -= Time.deltaTime;
                    if (time <= 0f)
                    {
                        enemy.ChangeState(new EnemyIdleState(enemy));
                    }
                }
            }
        }
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                enemy.ChangeState(new EnemyIdleState(enemy));
            }
        }
        
    }
}
