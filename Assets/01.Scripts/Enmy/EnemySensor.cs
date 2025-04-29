
using UnityEngine;



public class EnemySensor : MonoBehaviour
{
    private EnemyController enemyController;
    
    private float checkRadius; // 감지 범위
    private float checkAngle; // 감지 각도
    private float offsetAmount; // Raycast에 추가할 오프셋 크기
    
    public LayerMask detectionLayer; // 감지할 레이어
    public LayerMask obstacleLayer; // 장애물 레이어 (벽 등)

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public GameObject DetectEnemies()
    {
        if (!enemyController)
        {
            enemyController = GetComponentInParent<EnemyController>();
        }
        checkRadius = enemyController.checkRadius;
        checkAngle = enemyController.checkAngle;
        offsetAmount = enemyController.checkOffsetAmount;
        // 범위 내 모든 Collider 탐지
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, checkRadius, detectionLayer);

        foreach (Collider col in detectedColliders)
        {
            Vector3 directionToTarget = col.transform.position - transform.position;
            directionToTarget.y = 0; // y축으로 계산을 안하도록 설정 (2D 평면에서만 검사)

            // 감지 각도 내에 있는지 확인
            if (Vector3.Angle(transform.forward, directionToTarget) <= checkAngle / 2)
            {
                // 오프셋을 적용한 방향으로 Raycast
                RaycastHit hit;

                // 방향 벡터에 오프셋을 동시에 적용한 방향
                Vector3 leftOffsetDirection = Quaternion.Euler(0, -offsetAmount, 0) * directionToTarget;
                Vector3 rightOffsetDirection = Quaternion.Euler(0, offsetAmount, 0) * directionToTarget;

                // 좌측과 우측 오프셋 범위 내에서 장애물이 있는지 한 번에 체크
                if (!Physics.Raycast(transform.position, leftOffsetDirection, out hit, checkRadius, obstacleLayer) &&
                    !Physics.Raycast(transform.position, rightOffsetDirection, out hit, checkRadius, obstacleLayer))
                {
                    // 장애물이 없다면 적을 감지
                    
                    return col.gameObject;
                }
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        // 타겟이 감지된 경우
        if (DetectEnemies())
        {
            // 타겟의 위치에 맞춰 Gizmos 그리기
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, DetectEnemies().transform.position); // 타겟까지의 선
            // detectedTarget을 기준으로도 오프셋 시각화
            Vector3 directionToTarget = DetectEnemies().transform.position - transform.position;
            directionToTarget.y = 0; // y축으로 계산을 안하도록 설정 (2D 평면에서만 검사)

            // 오프셋을 적용한 방향으로 시각화
            Vector3 leftOffsetDirection = Quaternion.Euler(0, -offsetAmount, 0) * directionToTarget;
            Vector3 rightOffsetDirection = Quaternion.Euler(0, offsetAmount, 0) * directionToTarget;

            // 좌측 오프셋 시각화 (detectedTarget 기준)
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,transform.position + leftOffsetDirection);

            // 우측 오프셋 시각화 (detectedTarget 기준)
            Gizmos.DrawLine(transform.position,transform.position + rightOffsetDirection);
        }

        // 부채꼴 모양의 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // 부채꼴 모양 그리기 (각도와 거리)
        Vector3 leftBoundary = Quaternion.Euler(0, -checkAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, checkAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * checkRadius); // 왼쪽 경계
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * checkRadius); // 오른쪽 경계
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * checkRadius); // 중심선

        // 오프셋 범위 시각화
        Gizmos.color = Color.blue;

        // 좌측 오프셋 시각화
        Vector3 leftOffset = Quaternion.Euler(0, -offsetAmount, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftOffset * checkRadius);

        // 우측 오프셋 시각화
        Vector3 rightOffset = Quaternion.Euler(0, offsetAmount, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + rightOffset * checkRadius);
    }
}
