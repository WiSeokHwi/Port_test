using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;
    public InputSystem_Player playerInput;
    private Vector2 movement;
    private Vector2 rotation;
    private bool jumpInput;
    private bool shiftInput;
    private bool rollTriggered = false;
    private bool attack;
    private bool equip;
    
    private float lastShiftTapTime = -1f;
    private float doubleTapThreshold = 0.5f; // 더블탭 허용 시간 (초)
    
    
    // 현재 입력을 외부에서 가져갈 수 있게 프로퍼티 제공
    public PlayerInputCommend CurrentInput { get; private set; }

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        playerInput = new InputSystem_Player();
        playerInput.Enable();
    }

    void OnEnable()
    {
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
        playerInput.Player.Rotation.performed += OnRotate;
        playerInput.Player.Rotation.canceled += OnRotate;
        playerInput.Player.Jump.performed += (input) => { jumpInput = true; };
        playerInput.Player.Jump.canceled += (input) => { jumpInput = false; };
        playerInput.Player.Shift.performed += (input) => { shiftInput = true; };
        playerInput.Player.Shift.canceled += (input) => { shiftInput = false;};
        playerInput.Player.Shift.performed += OnShiftPerformed;
        playerInput.Player.Attack.performed += (input) => {attack = true; };
        playerInput.Player.Equip.performed += (input) => {equip = true; };
        
    }

    void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Player.Rotation.performed -= OnRotate;
        playerInput.Player.Rotation.canceled -= OnRotate;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 v = context.ReadValue<Vector2>();
        rotation = new Vector2(v.x, v.y);
    }
    private void OnShiftPerformed(InputAction.CallbackContext context)
    {
        float currentTime = Time.time;
    
        // 더블탭 감지
        if (currentTime - lastShiftTapTime < doubleTapThreshold)
        {
            rollTriggered = true;
        }

        lastShiftTapTime = currentTime;
        shiftInput = true;
    }

    void Update()
    {
        CurrentInput  = new PlayerInputCommend( // PlayerInputCommend 을 거쳐서 input값을 한번에 보냄
            movement,
            rotation,
            jumpInput,
            attack,
            shiftInput,
            equip,
            rollTriggered
            
        );
        
        playerController.HandleInput(CurrentInput);//HandleInput 으로 CurrentInput 값을 보냄
        rollTriggered = false;
        attack = false;
        equip = false;
    }
    
    
}
