using UnityEngine;

public class AgentBehaviour : MonoBehaviour
{
    public GameObject target; //타겟 게임오브젝트
    protected Agent agent; //Agent 스크립트
    
    public float maxSpeed;
    public float maxAccel;
    public float maxRotation;
    public float maxAngularAccel;
    

    public virtual void Awake()
    {
        agent = gameObject.GetComponent<Agent>(); // 오브젝트의 Agent 를 가져옴
    }

    public virtual void Update()
    {
        agent.SetSteering(GetSteering()); // agent.SetSteering을 통하여 행위를 업데이트
    }

    public virtual Steering GetSteering() // 행위에서 실행시켜 행위를 새로 받아오는 함수.
    {
        return new Steering(); 
    }

    public float MapToRange(float rotation) // 회전값 정규화 -180도 ~ 180도 사이로
    {
        rotation %= 360.0f;
        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f) rotation += 360.0f;
            else rotation -= 360.0f;
        }
        return rotation;
    }

    public Vector3 GetOriAsVec(float orientation) // 방향을 찾는 메소드
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1.0f ;
        vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1.0f ;
        return vector.normalized;
    }
}
