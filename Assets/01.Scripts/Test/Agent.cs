using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxSpeed; // 최대 속도
    public float maxAccel; // 최대 가속도 (부드러운 움직임을 위해)
    public float maxRotation; // 최대 회전값을 제한하는 변수
    public float orientation; // 현제 회전값을 저장하는 변수
    public float rotation; // 회전량 변수
    public Vector3 velocity; // 이동값 변수

    public float maxAngularAccel; // 최대 회전 가속도
    
    protected Steering steering; // 동작을 담을 변수
    
    private Rigidbody aRigidbody; // 리지드바디를 담을 변수
    void Start()
    {
        velocity = Vector3.zero; // 속도 초기화
        steering = new Steering(); // 동작 초기화
        aRigidbody = GetComponent<Rigidbody>(); // 리지드바디 불러오기
    }

    public virtual void Update() 
    {
        if (aRigidbody == null) //리지드바디가 없다면 리턴
        {
            return;
        }
        
        Vector3 displacement = velocity * Time.deltaTime; // 이동속도, 방향을 변수에 저장
        orientation += rotation * Time.deltaTime; // 입력된 회전값을 현재 회전에 더함
        
        //회전 값들의 범위를 0에서 360 사이로 제한
        if (orientation < 0.0f) orientation += 360.0f; // 현재회전정도가 0보다 작다면 360으로
        else if (orientation > 360.0f) orientation -= 360.0f; // 현재 회전 정도가 360보다 크다면 0으로
        transform.Translate(displacement, Space.World); // displacement 만큼 월드좌표기준으로 이동
        transform.rotation = new Quaternion(); // 기존 회전값 초기화
        transform.Rotate(Vector3.up, orientation); // orientation 만큼 회전
    }

    public virtual void FixedUpdate()
    {
        if (aRigidbody == null) //리지드 바디가 없다면 리턴
        {
            return;
        }
        Vector3 displacement = velocity * Time.deltaTime; // 이동 속도, 방향 을 저장
        orientation += rotation * Time.deltaTime; // 입력된 회전값을 현제 회전값에 반영
        
        // 회전 최대 최소 제한
        if (orientation < 0.0f) orientation += 360.0f;
        else if (orientation > 360.0f) orientation -= 360.0f;
        // 무엇을 하고 싶은지에 따라 포스 모드 (ForcMode) 값을 설정한다
        // 여기서는 보여주는 용도로 VelocityChange를 사용한다
        aRigidbody.AddForce(displacement, ForceMode.VelocityChange); // AddForce를 이용하여 물리이동
        Vector3 orientationVector = OriToVec(orientation); // orientation 값을 OriToVec에서 변환하여 변수에 대입
        aRigidbody.rotation = Quaternion.LookRotation(orientationVector, Vector3.up);//y 축으로 orientationVector을 바랍도록 회전
    }

    public virtual void LateUpdate()
    {
        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }

        if (steering.angular == 0.0f)
        {
            rotation = 0.0f;
        }

        if (steering.linear.sqrMagnitude == 0.0f)
        {
            velocity = Vector3.zero;
        }

        steering = new Steering();
    }

    public Vector3 OriToVec(float orientation) // float 값을 받아옴
    {
        Vector3 vector = Vector3.zero; // 초기화 벡터를 생성 
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f; // 받아온 orientation 값을 Sin, Cos을 통하여 orientation 이 바라보는 방향으로 변경
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;// nomalized를 하여 값을 반환
    }
    

    public void SetSteering(Steering steering)
    {
        this.steering = steering;
    }
}
