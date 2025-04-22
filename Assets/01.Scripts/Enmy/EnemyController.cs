using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyState currentState { get; private set; }
    public GameObject Target;


    void Awake()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
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
    
    public void MoveTowards(Vector3 targetPos)
    {
        // 이동 코드
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        
        currentState.Enter();
    }
}

