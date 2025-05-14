using UnityEngine;

public class Flee : AgentBehaviour // 도망가는 행위
{
    public override Steering GetSteering()
    {
        Steering steering = new Steering(); // 새로운 Flee 행위 템플릿 생성
        steering.linear = target.transform.position - transform.position; // steeting의 linear를 타겟위치 - 내위치 = 타켓쪽 방향 
        steering.linear.Normalize(); // 벡터를 1로 만들어 방향만 유지
        steering.linear = steering.linear * agent.maxAccel; // 방향 * 가속도 
        return steering;
    }
}
 