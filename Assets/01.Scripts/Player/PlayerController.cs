
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour {
    
    public IPlayerState _currentState { get; private set; }
    public Animator _animator;
    public Vector3 LastMoveDirection;
    
    public CharacterController controller;
    public Transform cameraTransform;
    private Health health;
    
    public Weapon CurrentWeapon;
    
    
    public float gravity = 9.8f;
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float jumpSpeed = 2f;
    
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool equipped  = false;
    public bool isCloaking = false;
    private int Equip;

    public Vector3 movePosition;

    [SerializeField] private GameObject WeaponPoseR;
    [SerializeField] private GameObject DrawWeaponPoseR;
    [SerializeField] public Rig HeadOverray;
    private GameObject weapone;
    


    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        controller  = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        
        health.OnHealthChanged += OnTakeDamage;
        health.OnDie += OnDeath;
        
        ChangeState(new PlayerIdleState());
        
        Equip = Animator.StringToHash("Equip");
        weapone = Instantiate(CurrentWeapon.weaponPrefab, DrawWeaponPoseR.transform);
        
    }
    
    
    void Update()
    {
        
        _currentState?.Update();
        movePosition.y  = GetYVelocity();
        controller.Move(movePosition * Time.deltaTime);
        Debug.Log("현재 상태" + _currentState);
        Debug.Log("땅" + controller.isGrounded);
        
        
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

    public void IsJumping()
    {
        isJumping = true;
    }

    private float GetYVelocity()
    {
        if (!controller.isGrounded)
        {
            return movePosition.y - gravity * Time.deltaTime;
        }

        if (isJumping)
        {
            isJumping = false;
            movePosition = LastMoveDirection * jumpSpeed;
            return movePosition.y + jumpSpeed;
        }
        else
        {
            return -0.5f;
        };
    }
    
    public void HandleInput(PlayerInputCommend input)
    {
        _currentState?.HandleInput(input);
    }

    /*public void Jump() {
        Vector3 jumpVelocity = LastMoveDirection * 3f + Vector3.up * jumpSpeed;
        movePosition.y += jumpVelocity.y;
        LastMoveDirection = Vector3.zero;
    }*/

    

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

    // 데미지 받았을때 실행될 메소드 Health.OnHealthChanged 구독
    void OnTakeDamage(float damage)
    {
        ChangeState(new PlayerHitState());
    }

    void OnDeath()
    {
        
    }
}
