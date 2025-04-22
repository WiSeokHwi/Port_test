using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerController playerController;
    public LayerMask groundLayer;
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
            Physics.Raycast(transform.position, Vector3.down, 0.02f, groundLayer),
            Input.GetKeyDown(KeyCode.T),
            Input.GetKeyDown(KeyCode.LeftShift)
    
        );
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 0.02f);
        playerController.HandleInput(input);
        
    }
    
    
}
