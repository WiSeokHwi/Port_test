using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    private IPlayerState _currentState;
    
    private Rigidbody rb;
    public float moveSpeed = 3f;
    public float runSpeed = 1.5f;

    void Awake()
    {
        
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
         
        rb.MovePosition(moveDirection * (moveSpeed * Time.deltaTime));
    }

    public void Jump() {
        // 점프 물리 처리
    }

    public bool IsGrounded() {
        // 바닥 체크 로직
        return true;
    }

    public void SetAnimation(string animName) {
        // 애니메이션 실행
    }
}