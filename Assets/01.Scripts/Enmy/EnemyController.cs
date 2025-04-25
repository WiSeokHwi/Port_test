using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyState currentState { get; set; }
    [SerializeField] private Transform target;
    public Transform Target => target;
    [SerializeField] private NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    [SerializeField] private Transform homePosition;
    public Transform HomePosition => homePosition;
    
    public Transform[] waypoints;
    private EnemyDetection detection;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform;
        detection = GetComponent<EnemyDetection>();
    }
    void Start()
    {
        currentState = new EnemyIdleState(this);
        currentState.Enter();
    }

    void Update()
    {
        currentState.Update();
        
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

