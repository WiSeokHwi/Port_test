using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    private IPlayerState _currentState;
    public Animator _animator;
    
    public Rigidbody rb;
    public float moveSpeed = 1.3f;
    public float runSpeed = 1.5f;
    public float jumpSpeed = 3.5f;
    public Vector3 LastMoveDirection { get; private set; }
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(new PlayerIdleState());
        
    }
    
    
    void Update()
    {
        _currentState?.InputHandler();
    }
    void FixedUpdate()
    {
        _currentState?.PhysicsUpdate();
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ChangeState(IPlayerState newState) {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    public void Move(Vector3 moveDirection)
    {
         
        rb.MovePosition(rb.position + moveDirection);
        LastMoveDirection = moveDirection.normalized;
        
    }

    public void Jump() {
        Vector3 jumpVelocity = LastMoveDirection * 3f + Vector3.up * jumpSpeed;
        rb.AddForce(jumpVelocity);
    }

    public bool IsGrounded()
    {
        float checkDistance = 0.1f; // 발 밑 거리
        return Physics.Raycast(transform.position, Vector3.down, checkDistance);
    }

    public void SetAnimation(string animName) {
        // 애니메이션 실행
    }
}
