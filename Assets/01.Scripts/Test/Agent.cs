using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float maxSpeed;
    public float maxAccel;
    public float orientation;
    public float rotation;
    public Vector3 velocity;
    
    protected Steering steering;
    
    private Rigidbody aRigidbody;
    void Start()
    {
        velocity = Vector3.zero;
        steering = new Steering();
        aRigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Update()
    {
        if (aRigidbody == null)
        {
            return;
        }
        
        Vector3 displacement = velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;
        //회전 값들의 범위를 0에서 360 사이로 제한
        if (orientation < 0.0f) orientation += 360.0f;
        else if (orientation > 360.0f) orientation -= 360.0f;
        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientation);
    }

    public virtual void FixedUpdate()
    {
        if (aRigidbody == null)
        {
            return;
        }
        Vector3 displacement = velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;
        if (orientation < 0.0f) orientation += 360.0f;
        else if (orientation > 360.0f) orientation -= 360.0f;
        // 무엇을 하고 싶은지에 따라 포스 모드 (ForcMode) 값을 설정한다
        // 여기서는 보여주는 용도로 VelocityChange를 사용한다
        aRigidbody.AddForce(displacement, ForceMode.VelocityChange);
        Vector3 orientationVector = OriToVec(orientation);
        aRigidbody.rotation = Quaternion.LookRotation(orientationVector, Vector3.up);
    }

    public virtual void LateUpdate()
    {
        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity = velocity * maxSpeed;
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

    public Vector3 OriToVec(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f;
        return vector.normalized;
    }
    

    public void SetSteering(Steering steering)
    {
        this.steering = steering;
    }
}
