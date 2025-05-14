using System;
using Mono.Cecil;
using UnityEngine;

public class Pursue : Seek // 추격하는 행위
{
    
    public float maxPrediction;
    private GameObject targetAux; // 타겟 오브젝트
    private Agent targetAgent; // 타겟의 Agent를 가져옴

    public override void Awake()
    {
        base.Awake();
        targetAgent = target.GetComponent<Agent>();
        targetAux = target; // 기존타겟을 저장
        target = new GameObject(); // 가짜 타겟 생성
    }

    private void OnDestroy() // 행위가 사라질때 targetAux도 파괴
    {
        Destroy(targetAux);
    }

    public override Steering GetSteering()
    {
        Vector3 direction = targetAux.transform.position - transform.position; // 타겟과 나의 방향 구하기
        float distance = direction.magnitude; // 목표와의 거리를 저장
        float speed = agent.velocity.magnitude; // 순수 이동속도를 담음
        float prediction; //
        if (speed <= distance / maxPrediction) prediction = maxPrediction; // 내가 느리면 maxPrediction만큼 예측
        else prediction = distance / speed; // 내가 더 빠르다면 거리 / 속도 만큼 예측
        
        target.transform.position = targetAux.transform.position;
        target.transform.position += targetAgent.velocity * prediction;
        
        return base.GetSteering();
    }
}
