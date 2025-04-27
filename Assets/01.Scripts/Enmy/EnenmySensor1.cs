using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class EnenmySensor1 : MonoBehaviour
{
    public Color meshColor = Color.red; //메쉬 색상
    public float distance =10f; // 감지 거리
    public float angle = 30f; //감지 각도
    public float height = 1f; // 감지 높이
    public float lower = 0f;
    public int scanFrequency = 30; // 초당 감지할 횟수
    public LayerMask layerMask; // 감지할 레이어마스크
    public LayerMask checkLayer; // 플레이어 감지 마스크
    public List<GameObject> Objects = new List<GameObject>(); // 센서에 감지된 오브젝트를 담을 리스트

    Collider[] colliders = new Collider[50]; //매번 새롭게 만들면 메모리 부하가 생기기때문에 50개로 제한 하고 미리생성
    private Mesh mesh; // 메쉬변수 생성

    private int count; // 감지된 콜라이더 의 갯수
    float scanInterval; // 감지주기 (얼마마다 감지할지 계산한 값)
    float scanTimer; // 감지 타이머 , 시간이 감소하여 0이 되면 Scan 하도록

    void Start()
    {
        scanInterval = 1.0f / scanFrequency; // scanInterval = 1.0 / 30 = 0.033초마다 1번 감지
        layerMask = LayerMask.GetMask("Ground", "Obstacle");
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0)
        {
            scanTimer += scanInterval;
            Scan();
            mesh = CreateWedgeMesh();
        }
        
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc( //NonAlloc 으로 새로 할당하지 않고 미리만들어진 자리(50개) 에 채움
            transform.position, // 중심 위치 ( 자신)
            distance, // 반지름 (감지 거리)
            colliders, // 감지된 콜라이더를 담을 배열
            checkLayer, // 감지할 대상의 레이어
            QueryTriggerInteraction.Collide); // 트리거 포함 여부
        Objects.Clear(); // 초기화
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject; // 감지된 콜라이더의 게임오브젝트 가져오기
            if (IsInSight(obj)) // 시야내에 있는지 검사
            {
                Objects.Add(obj); // 시야 내에 있으면 Objects 배열에 추가
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position; // 센서 중심 위치
        Vector3 dest = obj.transform.position;
        Vector3 direction = (dest - origin).normalized; // 타겟 방향 벡터

        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0f;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        origin.y = height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, layerMask))
        {
            return false;
        }
        
        
        return true;
    }
    
    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh(); // 메쉬 생성

        
        int segment = 10; // 분할 수 *부채꼴을 부드럽게 만들기 위함
        int numTriangles = (segment * 4) + 2 + 2; // (분할 수 * (전면(2개) + 윗면(1개) + 아랫면(1개))) + (좌측 2개 + 우측 2개)
        int numVertices = numTriangles * 3; // 삼각형의 정점은 3개이기 때문
        
        Vector3[] vertices = new Vector3[numVertices]; // 정점의 갯수만큼 좌표를 생성
        int[] triangles = new int[numVertices]; // 버텍스 수와 같게 삼각형 정점 리스트 생성
        
        
        // 버텍스가 위치해야할 위치 생성 
        
        //하단 삼각 좌표
        
        Vector3 leftDirection = Quaternion.Euler(0, -angle, 0) * Vector3.forward; // 좌측 하단 좌표
        Vector3 rightDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward; // 하단 우측 좌표
        
        Vector3 bottomLeft;
        Vector3 bottomRight;
        Vector3 bottomCenter = transform.position + new Vector3(0f, lower, 0f); // 하단 중앙 좌표
        
        // raycast Left
        if (Physics.Raycast(bottomCenter, leftDirection, out RaycastHit leftHit, distance, layerMask))
        {
            
            
            bottomLeft = leftHit.point;
        }
        else
        {
            bottomLeft = bottomCenter + leftDirection * distance; // 좌측 하단 좌표
        }
        
        // raycast Right
        if (Physics.Raycast(bottomCenter, rightDirection, out RaycastHit rightHit, distance, layerMask))
        {
            bottomRight = rightHit.point;
        }
        else
        {
            bottomRight = bottomCenter + rightDirection * distance; // 좌측 하단 좌표
        }
        
        // 상단 삼각 좌표
        Vector3 topCenter = transform.position + Vector3.up * height; // 상단 중앙 좌표
        Vector3 topLeft;
        Vector3 topRight;
        
        // raycast Left
        if (Physics.Raycast(topCenter, leftDirection, out RaycastHit LT_Hit, distance, layerMask))
        {
            topLeft = LT_Hit.point;
        }
        else
        {
            topLeft = topCenter + leftDirection * distance; // 좌측 하단 좌표
        }
        
        // raycast Right
        if (Physics.Raycast(topCenter, rightDirection, out RaycastHit RT_Hit, distance, layerMask))
        {
            topRight = RT_Hit.point;
        }
        else
        {
            topRight = topCenter + rightDirection * distance; // 좌측 하단 좌표
        }

        int vert = 0; // 버텍스 인덱스별 좌표 설정을 위한 변수
        
        // left side 삼각형 만들기 2개
        vertices[vert++] = bottomCenter; // 버텍스 위치를 옮기고 vert 카운트
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;
        
        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;
        
        
        // right side 삼각형 만들기 2개
        
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        
        vertices[vert++] = topRight;
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        
        float currenAngle = -angle; // 시작점 
        float deltaAngle = (angle * 2) / segment; // 감지 각도를 세그먼트로 나눈 값
        
        for (int i = 0; i < segment; ++i) // 세그먼트 만큼 반복 생성
        {
            
            leftDirection = Quaternion.Euler(0, currenAngle, 0) * Vector3.forward;
            rightDirection = Quaternion.Euler(0, currenAngle + deltaAngle, 0) * Vector3.forward;
            
            // bottomLeft raycast
            if (Physics.Raycast(bottomCenter, leftDirection, out RaycastHit bottomLeftHit, distance, layerMask))
            {
                Debug.Log("bottomLeft");
                bottomLeft = bottomLeftHit.point;
            }
            else
            {
                bottomLeft = bottomCenter + leftDirection * distance;
            }

// bottomRight raycast
            if (Physics.Raycast(bottomCenter, rightDirection, out RaycastHit bottomRightHit, distance, layerMask))
            {
                bottomRight = bottomRightHit.point;
            }
            else
            {
                bottomRight = bottomCenter + rightDirection * distance;
            }
            
            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;
            
            
            // far side 삼각형 만들기 2개
        
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;
        
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
        
            // top 삼각형 만들기 1개
        
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;
        
            // bottom 삼각형 만들기 1개
        
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
            
            
            currenAngle += deltaAngle; // 시작 각도를 수정
        }
        
        for (int i = 0; i < numVertices; ++i) // vertices 인덱스 번호와 triangles 인덱스 번호 맞추기
        {
            triangles[i] = i;
        }
        
        
        mesh.vertices = vertices; // verices 생성
        mesh.triangles = triangles; // 삼각형 생성
        mesh.RecalculateNormals(); // RecalculateNormals() 스크립트로 메쉬를 조작시 노말이 수정되지 않아 맞춰주는 함수
        
        return mesh;
    }

    private void OnValidate() // 유니티 에디터에서 값이 변경되면 호출되는 함수 
    {
        
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh);
        }
        Gizmos.DrawWireSphere(transform.position,distance);
        for (int i = 0; i < count; ++i)
        {
            Gizmos.DrawSphere(colliders[i].transform.position,0.5f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {
            Gizmos.DrawSphere(obj.transform.position,0.5f);
        }
    }

}
