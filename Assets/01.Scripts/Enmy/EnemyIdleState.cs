
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
        enemy.SetTarget(enemy.HomePosition);
        agent.SetDestination(enemy.Target.transform.position);
        agent.speed = enemy.walkSpeed;
        cloakingCheckAmount = enemy.cloakingCheckAmount;

    }

    public override void Update()
    {
        DrawDetectionRays();
        ChangeCheckState();
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
                    //  -offsetAmount부터 +offsetAmount까지, 일정 간격으로 체크
                    int offsetChecks = 5; // 체크할 횟수 (숫자가 클수록 촘촘)
                
                    for (int i = 0; i <= offsetChecks; i++)
                    {
                        // -offsetAmount부터 +offsetAmount까지 균등 분포
                        float lerpFactor = (float)i / offsetChecks; // 0 ~ 1 사이
                        float offsetAngle = Mathf.Lerp(-enemy.checkOffsetAmount, enemy.checkOffsetAmount, lerpFactor); // a, b 를 c (0~1) 값을 비율로

                        Vector3 offsetDirection = Quaternion.Euler(0, offsetAngle, 0) * dirToTarget; // 
                        
                        
                        
                        RaycastHit hit;
                        
                        if (!Physics.Raycast(enemy.transform.position, offsetDirection, out hit, enemy.checkRadius, layerMask))
                        {
                            enemy.SetTarget(target);
                            enemy.ChangeState(new EnemyCheckState(enemy));
                        }
                    }
                }
            }
        }
    }
    private void DrawDetectionRays()
    {
        int rayCount = 20; // 부채꼴의 세밀함 (숫자가 클수록 촘촘함)
        float halfAngle = enemy.checkAngle * 0.5f;
        Vector3 forward = enemy.transform.forward;

        for (int i = 0; i <= rayCount; i++)
        {
            float lerp = (float)i / rayCount;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, lerp);

            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * forward;

            Debug.DrawRay(enemy.transform.position, direction * enemy.checkRadius, Color.yellow);
        }
        
        int offsetChecks = 5;

        for (int i = 0; i <= offsetChecks; i++)
        {
            float lerpFactor = (float)i / offsetChecks;
            float offsetAngle = Mathf.Lerp(-enemy.checkOffsetAmount, enemy.checkOffsetAmount, lerpFactor);

            Vector3 offsetDirection = Quaternion.Euler(0, offsetAngle, 0) * forward;

            Debug.DrawRay(enemy.transform.position, offsetDirection * enemy.checkRadius, Color.green);
        }
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
            enemy.SetTarget(enemy.waypoints[pathIndex]);
            agent.SetDestination(enemy.Target.transform.position);
            timer = 0f;
    }
}
