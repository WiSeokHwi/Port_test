using UnityEngine;

public class EnemyCheckState : EnemyState
{
    public EnemyCheckState(EnemyController enemy) : base(enemy) {}
    private Vector3 detectedPosition; // 감지했을 때 플레이어 위치 저장
    private float timer = 0;
    private float waitTime = 1.5f;
    private LayerMask detectionLayer = LayerMask.GetMask("Player");
    private LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
    private float detectionRadius;

    public override void Enter()
    {
        base.Enter();
        detectedPosition = enemy.Target; // 감지 당시 위치를 저장
        agent.SetDestination(detectedPosition);   // 그 위치로 이동
        agent.speed = enemy.walkSpeed;
        detectionRadius = enemy.detectionRadius;
    }

    public override void Update()
    {
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
