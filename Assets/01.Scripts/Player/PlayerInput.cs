using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerController playerController;

    void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
        
    }
    void Update()
    {
        
        var input = new PlayerInputCommend(
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            Input.GetKeyDown(KeyCode.Space),
            Input.GetMouseButtonDown(0),
            Input.GetKey(KeyCode.LeftShift),
            Physics.Raycast(transform.position, Vector3.down, 0.1f),
            Input.GetKeyDown(KeyCode.T),
            Input.GetKeyDown(KeyCode.LeftShift)
    
        );

        playerController.HandleInput(input);
        Debug.Log("땅에 닿았는가? :" + input.IsGrounded);
    }
}
