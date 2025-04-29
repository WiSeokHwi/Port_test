using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    private Animator animator;
    
    private EnemyState currentState { get; set; }
    [SerializeField] private GameObject target;
    public GameObject Target => target;
    
    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    [SerializeField] private GameObject homePosition;
    public GameObject HomePosition => homePosition;

    public EnemySensor enenmySensor;
    
    public Transform[] waypoints;

    public float detectionRadius = 6f;
    public float checkRadius = 8f;
    public float checkAngle = 30f;
    public float checkOffsetAmount = 0.5f;
    public float cloakingCheckAmount = 3f;
    public float walkSpeed = 1.2f;
    public float runSpeed = 2.5f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enenmySensor = GetComponentInChildren<EnemySensor>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        currentState = new EnemyIdleState(this);
        currentState.Enter();
    }

    void Update()
    {
        Debug.Log(Agent.velocity.magnitude);
        float speedPercent = 0f;

        if (Agent.velocity.magnitude <= walkSpeed)
        {
            // Walk 구간: 0 ~ 0.5로 매핑
            speedPercent = Mathf.InverseLerp(0f, walkSpeed, Agent.velocity.magnitude) * 0.5f;
        }
        else
        {
            // Run 구간: 0.5 ~ 1.0로 매핑
            speedPercent = 0.5f + Mathf.InverseLerp(walkSpeed, runSpeed, Agent.velocity.magnitude) * 0.5f;
        }

        animator.SetFloat("MoveSpeed", speedPercent, 0.1f, Time.deltaTime);
        currentState.Update();
        Debug.Log("적 상태 : " + currentState);
    }

    void FixedUpdate()
    {   
        currentState.PhysicsUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        
        currentState.Enter();
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, cloakingCheckAmount);
    }
}

