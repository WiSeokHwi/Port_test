using UnityEngine;

public class PlayerController : MonoBehaviour {
    private IPlayerState _currentState;

    void Start() {
        ChangeState(new PlayerIdleState());
    }

    void Update() {
        _currentState?.Update();
    }

    public void ChangeState(IPlayerState newState) {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    public void Move(float direction) {
        transform.Translate(Vector3.right * direction * Time.deltaTime * 5f);
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