using UnityEngine;

// 이 스크립트를 붙일 오브젝트 또는 Prefab에 MeshFilter와 MeshRenderer가 필요
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))] 
public class AttackConeVisualizer : MonoBehaviour
{
    // Inspector에서 설정할 변수
    [Header("Mesh Settings")]
    [SerializeField] private Mesh attackConeMeshTemplate; // 사용할 기본 격자 메쉬 (Prefab에서 가져올 수도 있음)
    [SerializeField] private int attackConeResolution = 20; // 메쉬 해상도 (격자 메쉬 사용 시에는 불필요할 수 있음)

    [Header("Terrain/Collider Settings")]
    [SerializeField] private LayerMask groundLayer; // 지형 콜라이더 Layer Mask
    [SerializeField] private float raycastCheckHeight = 5f; // 레이캐스트 시작 높이 오프셋
    [SerializeField] private float meshYOffsetAboveGround = 0.05f; // 바닥에서 띄울 높이

    [Header("Fade Settings")]
    [SerializeField] private float vizFadeSpeed = 3f; // 페이드 속도

    // 내부 사용 변수
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh currentMesh; // 런타임에 사용할 메쉬 인스턴스
    private Vector3[] baseVertices; // 초기 (평평한) 메쉬 정점들

    private float currentVizAlpha = 0f;
    private float targetVizAlpha = 0f;

    // EnemyController로부터 받아올 동적 정보
    // EnemyController가 매 프레임 이 값을 설정해 줍니다.
    public Vector3 EnemyPosition { get; set; }
    public Vector3 EnemyForward { get; set; }
    public float AttackRange { get; set; }
    public float AttackAngle { get; set; }

    // 시각화 메쉬에 사용할 Material (셰이더 변수 제어용)
    private Material vizMaterial;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null || meshRenderer == null)
        {
            Debug.LogError("AttackConeVisualizer requires MeshFilter and MeshRenderer components.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        // 메쉬 데이터 초기화
        if (attackConeMeshTemplate != null)
        {
            // 원본 메쉬 데이터 복제 (여러 적이 같은 Prefab 사용할 때 필수)
            currentMesh = Instantiate(attackConeMeshTemplate); 
            meshFilter.mesh = currentMesh;
            baseVertices = currentMesh.vertices; // 기본 정점 위치 저장
        }
        else if (meshFilter.mesh != null)
        {
             // Prefab 자체에 메쉬가 있다면 복제
             currentMesh = Instantiate(meshFilter.mesh);
             meshFilter.mesh = currentMesh;
             baseVertices = currentMesh.vertices;
        }
        else
        {
            Debug.LogError("AttackConeVisualizer: No Mesh Template assigned and no mesh on MeshFilter!");
            enabled = false;
            return;
        }

        // Material 인스턴스 생성 (Material 공유 방지)
        vizMaterial = meshRenderer.material = new Material(meshRenderer.material);

        // 초기 투명도 설정
        vizMaterial.SetFloat("_Visibility", currentVizAlpha);

        // 처음에 보이지 않도록 비활성화 (필요에 따라)
        // gameObject.SetActive(false); 
        // 또는 알파값 0으로 제어하므로 활성화 상태로 둬도 무방
    }

    void Update()
    {
        // 적 위치와 회전을 따라가도록 설정 (EnemyController가 자식으로 붙여줬다면 불필요)
        // transform.position = EnemyPosition; // 부모 위치 따라감
        // transform.rotation = Quaternion.LookRotation(EnemyForward); // 부모 회전 따라감

        // 지형 높이 맞추기 업데이트 (EnemyController가 설정해준 최신 정보를 사용)
        UpdateConeMeshHeightFromColliders(); // 또는 UpdateConeMeshHeight (Terrain용) 사용

        // 셰이더 변수 전달 (EnemyController가 설정해준 최신 정보를 사용)
        if (vizMaterial != null)
        {
            vizMaterial.SetVector("_EnemyPosition", EnemyPosition); 
            vizMaterial.SetVector("_EnemyForward", new Vector4(EnemyForward.x, 0, EnemyForward.z, 0));
            vizMaterial.SetFloat("_AttackRangeSq", AttackRange * AttackRange); 
            vizMaterial.SetFloat("_AttackRange", AttackRange); 
            vizMaterial.SetFloat("_HalfAttackAngleRad", (AttackAngle / 2f) * Mathf.Deg2Rad);

            // 시각화 투명도 부드럽게 변경 로직
            if (currentVizAlpha != targetVizAlpha)
            {
                currentVizAlpha = Mathf.MoveTowards(currentVizAlpha, targetVizAlpha, vizFadeSpeed * Time.deltaTime);
                vizMaterial.SetFloat("_Visibility", currentVizAlpha);

                // 페이드아웃 완료 시 오브젝트 비활성화 (선택 사항, 성능)
                if (targetVizAlpha <= 0.01f && currentVizAlpha <= 0.01f)
                {
                   // gameObject.SetActive(false);
                }
            }
        }
    }

    // 시각화 페이드인 시작
    public void StartFadeIn()
    {
        targetVizAlpha = 1f;
        // gameObject.SetActive(true); // 비활성화했다면 다시 활성화
    }

    // 시각화 페이드아웃 시작
    public void StartFadeOut()
    {
        targetVizAlpha = 0f;
    }

    // 메쉬 정점 높이를 지형(콜라이더)에 맞춰 업데이트하는 함수 (이전 EnemyController에서 이동)
    private void UpdateConeMeshHeightFromColliders()
    {
        if (currentMesh == null || baseVertices == null || baseVertices.Length == 0) return;

        Vector3[] currentVertices = currentMesh.vertices;
        Transform vizTransform = transform; // 이 스크립트가 붙은 오브젝트의 Transform

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 baseLocalPos = baseVertices[i];

            // 레이캐스트 시작 지점 계산 (시각화 메쉬 오브젝트 로컬 XZ를 월드로 변환 후 높이 오프셋 추가)
            Vector3 flatWorldPos = vizTransform.TransformPoint(new Vector3(baseLocalPos.x, 0, baseLocalPos.z));
            Vector3 rayOrigin = flatWorldPos + Vector3.up * raycastCheckHeight;

            RaycastHit hit;
            // 레이캐스트 실패 시 사용할 기본 높이 (적 발 밑보다 약간 아래 또는 현재 오브젝트 Y)
            float groundHeight = vizTransform.position.y - 1.0f; 

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastCheckHeight * 2f, groundLayer))
            {
                groundHeight = hit.point.y;
            }

            // 얻어온 groundHeight를 메쉬 정점의 로컬 Y 좌표로 변환
            // 정점 로컬 Y = (실제 월드 높이) - (시각화 메쉬 오브젝트 월드 Y) + (바닥에서 띄울 오프셋)
            float localY = (groundHeight - vizTransform.position.y) + meshYOffsetAboveGround;

            currentVertices[i] = new Vector3(baseLocalPos.x, localY, baseLocalPos.z);
        }

        currentMesh.vertices = currentVertices;
        currentMesh.RecalculateBounds();
    }

    // Terrain.SampleHeight를 사용하고 싶다면 이 함수를 사용 (groundLayer, raycastCheckHeight 등은 불필요)
    /*
    private void UpdateConeMeshHeightWithTerrain()
    {
         if (currentMesh == null || baseVertices == null || baseVertices.Length == 0) return;

         Vector3[] currentVertices = currentMesh.vertices;
         Transform vizTransform = transform; 
         Terrain activeTerrain = Terrain.activeTerrain; 

         if (activeTerrain == null)
         {
              for (int i = 0; i < baseVertices.Length; i++)
              {
                   currentVertices[i] = new Vector3(baseVertices[i].x, 0.01f, baseVertices[i].z); 
              }
         }
         else
         {
             for (int i = 0; i < baseVertices.Length; i++)
             {
                 Vector3 worldPos = vizTransform.TransformPoint(baseVertices[i]);
                 float terrainHeight = activeTerrain.SampleHeight(worldPos);
                 currentVertices[i] = new Vector3(baseVertices[i].x, (terrainHeight - vizTransform.position.y) + meshYOffsetAboveGround, baseVertices[i].z); 
             }
         }

         currentMesh.vertices = currentVertices;
         currentMesh.RecalculateBounds(); 
    }
    */

    // 오브젝트 파괴 시 메쉬 인스턴스도 파괴하여 메모리 누수 방지
    void OnDestroy()
    {
        if (currentMesh != null)
        {
            Destroy(currentMesh);
        }
        // Material 인스턴스도 파괴 (만약 Instantiate로 생성했다면)
        if (vizMaterial != null && meshRenderer != null && meshRenderer.sharedMaterial != vizMaterial)
        {
             Destroy(vizMaterial);
        }
    }
}