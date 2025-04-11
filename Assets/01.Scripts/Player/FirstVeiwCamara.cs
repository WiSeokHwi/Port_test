using System;
using UnityEngine;

public class FirstVeiwCamara : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 70f); // 고개 꺾임 제한

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // 좌우 회전 (플레이어 전체)
        player.Rotate(Vector3.up * mouseX);
    }
}
