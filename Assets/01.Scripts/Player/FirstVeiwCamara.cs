using System;
using UnityEngine;

public class FirstVeiwCamara : MonoBehaviour
{
    public Transform cameraTransform;      // Main Camera
    public float mouseSensitivity = 100f;


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
        xRotation = Mathf.Clamp(xRotation, -90f, 70f);  // 상하 회전 제한

        // 카메라 회전만 처리 (캐릭터는 회전하지 않음)
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f); // 캐릭터는 Y축만 회전

        
    }
}
