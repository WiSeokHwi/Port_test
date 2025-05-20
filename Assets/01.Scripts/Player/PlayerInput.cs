using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerController playerController;
    private GroundChecker _groundChecker;
    void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        _groundChecker = GetComponent<GroundChecker>();
        

    }
    void Update()
    {

        PlayerInputCommend input = new PlayerInputCommend(
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            Input.GetKeyDown(KeyCode.Space),
            Input.GetMouseButtonDown(0),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetKeyDown(KeyCode.T),
            Input.GetKeyDown(KeyCode.LeftShift)
            
        );
        playerController.HandleInput(input);
    }
    
    
}
