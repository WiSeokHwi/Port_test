using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyDetection : MonoBehaviour
{
    public float radius = 5f;        // 부채꼴의 반지름 (공격 거리)
    public int rayCount = 50;        // 부채꼴을 구성할 삼각형의 개수 (레이의 개수)
    public float angle = 90f;        // 부채꼴의 각도 (도 단위)
    public LayerMask obstacleMask;   // 장애물 감지 레이어 마스크 (여러 레이어 가능)
    public float heightOffset = 2f;  // 장애물 감지 시 높이 오프셋 (적의 높이나 기준점에서 얼마나 위나 아래로 감지할지)

    private Mesh mesh;               // 부채꼴을 위한 메쉬
    private Vector3[] vertices;      // 부채꼴의 정점들
    private int[] triangles;         // 부채꼴을 구성하는 삼각형의 인덱스들

    void Start()
    {
        mesh = new Mesh();  // 새 메쉬 객체 생성
        GetComponent<MeshFilter>().mesh = mesh;  // MeshFilter에 메쉬를 적용
        vertices = new Vector3[rayCount + 2];   // 정점 배열 생성 (중심점 + 외각)
        triangles = new int[rayCount * 3];      // 삼각형 배열 생성 (각각의 삼각형을 구성하는 3개의 인덱스)
    }

    void LateUpdate()
    {
        DrawFan3D();  // 부채꼴 업데이트
    }

    // 3D 부채꼴을 그리는 함수
    void DrawFan3D()
    {
        Vector3[] newVertices = new Vector3[rayCount + 2];
        int[] newTriangles = new int[rayCount * 3];

        newVertices[0] = Vector3.zero;  // 중심점 정점 (0,0,0)

        float startAngle = -angle / 2f;  // 부채꼴 시작 각도 (각도는 -angle / 2부터 시작)

        // 부채꼴의 외곽 정점들을 계산 (각도에 따라 레이캐스트 진행)
        for (int i = 0; i <= rayCount; i++)
        {
            // 각도를 계산해서 부채꼴을 형성하는 각도를 구합니다.
            float currentAngle = startAngle + angle * i / rayCount;  // 각도 계산

            // 레이의 방향을 구합니다. (Z축을 기준으로 회전하는 방향 벡터)
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            // 레이의 시작점은 (x, y, z) 기준 위치에 높이 오프셋을 더합니다.
            Vector3 rayOrigin = transform.position + Vector3.up * heightOffset;

            // 장애물이 없을 경우, 최대 반경까지 레이 캐스트
            Vector3 hitPoint = rayOrigin + dir * radius;

            // 레이캐스트를 쏘고 장애물이 있으면, 그 지점으로 hitPoint 업데이트
            if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, radius, obstacleMask))
            {
                hitPoint = hit.point;
            }

            // 로컬 좌표로 변환 후, 외곽 점으로 저장
            newVertices[i + 1] = transform.InverseTransformPoint(hitPoint);
        }

        // 삼각형 인덱스 설정 (부채꼴의 삼각형을 만들기 위해 각 점을 연결)
        for (int i = 0; i < rayCount; i++)
        {
            newTriangles[i * 3] = 0;           // 중심점
            newTriangles[i * 3 + 1] = i + 1;   // 현재 외곽 점
            newTriangles[i * 3 + 2] = i + 2;   // 다음 외곽 점
        }

        // 메쉬에 새로 계산된 정점과 삼각형을 할당
        mesh.Clear();
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.RecalculateNormals();  // 조명 처리를 위한 노멀 재계산
    }
}
