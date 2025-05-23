using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;
    public InputSystem_Player playerInput;
    private Vector2 movement;
    private Vector2 rotation;
    
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

    void Update()
    {
        CurrentInput  = new PlayerInputCommend(
            movement,
            rotation,
            Input.GetKeyDown(KeyCode.Space),
            Input.GetMouseButtonDown(0),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetKeyDown(KeyCode.T),
            Input.GetKeyDown(KeyCode.LeftShift)
            
        );
        playerController.HandleInput(CurrentInput);
    }
    
    
}
