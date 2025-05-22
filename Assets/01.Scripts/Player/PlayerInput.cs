using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;
    private InputSystem_Player playerInput;
    private Vector2 movement;
    private Vector2 rotation;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        playerInput = new InputSystem_Player();
        playerInput.Enable();
    }

    void Start()
    {
        playerInput.Player.Move.performed += (context) =>
        {
            movement = context.ReadValue<Vector2>();
        };
        playerInput.Player.Rotation.performed += (context) =>
        {
            Vector2 v = context.ReadValue<Vector2>();
            rotation = new Vector2(v.y, -v.x);
        };

    }
    void Update()
    {

        PlayerInputCommend input = new PlayerInputCommend(
            movement,
            rotation,
            Input.GetKeyDown(KeyCode.Space),
            Input.GetMouseButtonDown(0),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetKeyDown(KeyCode.T),
            Input.GetKeyDown(KeyCode.LeftShift)
            
        );
        playerController.HandleInput(input);
    }
    
    
}
