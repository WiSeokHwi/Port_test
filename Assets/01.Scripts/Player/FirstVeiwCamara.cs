using System;
using UnityEngine;

public class FirstVeiwCamara : MonoBehaviour
{
    
    public float mouseSensitivity = 100f;
    public Transform headRig;
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라의 좌우 회전
        yRotation += mouseX;  
        
        // 카메라의 상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 50f);  // 상하 회전 제한

        // 카메라 회전만 처리 (캐릭터는 회전하지 않음)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        

        
    }
}
