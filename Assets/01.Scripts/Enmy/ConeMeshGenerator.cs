using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class ConeMeshGenerator : MonoBehaviour
{
    public int segmentCount = 30;  // 분할 갯수
    public float radius = 5f;      // 사거리
    public float angle = 60f;      // 퍼지는 각도 (degree)
    public float rayHeight = 50f; // ray를 쏠 위치
    public float heightOffset = 0.1f; // 바닥에 뭍히지 않게 오프셋
    public LayerMask groundMask; // 감지할 마스크

    private LineRenderer lr;
    private Vector3[] points; // 감지한 좌표값 저장할 리스트

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;  // 루프값 초기화 (시작점과 끝점을 이어줌)
        lr.positionCount = segmentCount + 1;  // 세그먼트 + 시작점
        points = new Vector3[lr.positionCount]; // 분할 갯수에 맞게 좌표 생성
    }

    void Update()
    {
        DrawCone(); // 업데이트를 통해 실시간으로 변화
    }

    void DrawCone()
    {
        Vector3 center = transform.position; // 원뿔의 시작점
        Vector3 forward = transform.forward; // 기준 방향
        Quaternion rotation = Quaternion.Euler(0, -angle / 2f, 0); // 왼쪽 시작 각도

        points[0] = center; // 중심점 담기

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount; // 세그먼트에 따른 i 번째 비율 구하기
            float currentAngle = Mathf.Lerp(-angle / 2f, angle / 2f, t); // 각도를 선형 보간
            rotation = Quaternion.Euler(0, currentAngle, 0); // 현재 각도로 회전

            Vector3 dir = rotation * forward; // 회전된 방향
            Vector3 horizontalPos = center + dir * radius; // 방향에 범위값 저장

            Vector3 rayOrigin = horizontalPos + Vector3.up * rayHeight; // 레이를 쏠 높이 저장
            Debug.DrawRay(rayOrigin, Vector3.down * (rayHeight * 2), Color.red, 0f, false);

            // 레이를 사용하여 바닥을 감지
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayHeight * 2, groundMask))
            {
                points[i + 1] = new Vector3(hit.point.x, hit.point.y + heightOffset, hit.point.z); // 바닥에 맞춰 높이 조정
            }
            else
            {
                points[i + 1] = new Vector3(horizontalPos.x, center.y + heightOffset, horizontalPos.z); // 바닥에 맞지 않으면 높이 오프셋
            }
        }

        points[points.Length - 1] = points[0];  // 마지막 점은 시작점과 연결

        lr.SetPositions(points);  // 라인 렌더러에 점들을 설정하여 그려줌
    }
}
