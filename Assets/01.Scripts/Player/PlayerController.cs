using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public IPlayerState _currentState { get; private set; }
    public Animator _animator;
    public Vector3 LastMoveDirection;
    public Rigidbody rb;
    public Transform cameraTransform;
    
    public Weapon CurrentWeapon;
        
    public float moveSpeed = 4f;
    public float runSpeed = 2f;
    public float jumpSpeed = 10f;
    public bool isAttacking = false;
    public bool equipped  = false;
    private int Equip;

    [SerializeField] private GameObject WeaponPoseR;
    [SerializeField] private GameObject DrawWeaponPoseR;
    private GameObject weapone;
    


    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(new PlayerIdleState());
        
        Equip = Animator.StringToHash("Equip");
        weapone = Instantiate(CurrentWeapon.weaponPrefab, DrawWeaponPoseR.transform);
    }
    
    
    void Update()
    {
        
        _currentState?.Update();
        Debug.Log("현재 상태" + _currentState);
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

    public void WeaponEquip()
    {
        if (equipped)
        {
            equipped = false;
        }
        else
        {
            equipped = true;
        }
            
        _animator.SetBool(Equip, equipped); // 애니메이터 bool 파라미터 전환
    }

    public void CurrentWeaponInHand()
    {
        weapone.transform.SetParent(WeaponPoseR.transform);
        weapone.transform.localPosition = Vector3.zero;
        weapone.transform.localRotation = Quaternion.identity;
    }

    public void CurrentWeaponDraw()
    {
        weapone.transform.SetParent(DrawWeaponPoseR.transform);
        weapone.transform.localPosition = Vector3.zero;
        weapone.transform.localRotation = Quaternion.identity;
    }

    

    public void Jump() {
        Vector3 jumpVelocity = LastMoveDirection * 3f + Vector3.up * jumpSpeed;
        rb.AddForce(jumpVelocity, ForceMode.Impulse);
        LastMoveDirection = Vector3.zero;
    }

    public void HandleInput(PlayerInputCommend input)
    {
        _currentState?.HandleInput(input);
    }

    public void SetAnimation(string animName) {
        // 애니메이션 실행
    }
    public void AttackStart()
    {
        isAttacking = true;
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }
    

}
