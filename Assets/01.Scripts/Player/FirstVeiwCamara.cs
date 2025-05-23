using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class FirstVeiwCamara : MonoBehaviour
{
    public PlayerInput playerInputScript; // PlayerInput 컴포넌트를 참조
    private InputSystem_Player input;   
    public float mouseSensitivity = 30f;
    
    float xRotation = 0f;
    float yRotation = 0f;
    public PlayerController player;
    private CinemachineCamera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<CinemachineCamera>();
        player = thisCamera.Target.TrackingTarget.GetComponent<PlayerController>();
        playerInputScript = player.GetComponent<PlayerInput>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        Vector2 rotInput = playerInputScript.CurrentInput.RotationInput;
        
        // 마우스 입력
        float mouseX = rotInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = rotInput.y * mouseSensitivity * Time.deltaTime;

        // 카메라의 좌우 회전
        yRotation += mouseX;  
        
        // 카메라의 상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 50f);  // 상하 회전 제한

        // 카메라 회전만 처리 (캐릭터는 회전하지 않음)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        
    }
}
