using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<GameObject> nodes; // 노드들을 저장할 리스트 (에디터에서 연결)
    List<PathSegment> segments; // 내부에서 사용할 세그먼트 리스트

    void Start()
    {
        segments = GetSegments(); // 시작시  세그먼트 설정
    }

    public List<PathSegment> GetSegments() // 노드로부터 세그먼트 만드는 메서드
    {
        List<PathSegment> segments = new List<PathSegment>(); // 새로운 리스트 생성 (지역변수)
        int i;
        for (i = 0; i < nodes.Count - 1; i++) // 노드가 4개라면 총 3개의 선분 생성
        {
            Vector3 src = nodes [i].transform.position; //첫번째 노드의 위치값 저장
            Vector3 dst = nodes [i+1].transform.position; // 두번째 노드의 위치값 저장
            PathSegment segment = new PathSegment(src, dst); // 두 노드를 PathSegment 로 연결하고 
            segments.Add(segment); // 리스트에 추가
        }
        return segments;
    }
    
    /* ex ) segments = [
            PathSegment(A, B),
            PathSegment(B, C),
            PathSegment(C, D)
        ]*/

    public float GetParam(Vector3 position, float lastParam) // 현재 위치가 경로상에서 얼마나 떨어졌는지  길이 기준으로 수치화
    {
        // 위치 > 거리수치
        // 에이전트와 가장 가까운 세그먼트 찾기
        float param = 0f;
        PathSegment currentSegment = null;
        float tempParam = 0f; // 누적된 거리
        foreach (PathSegment ps in segments)  // segments (선분) 수 만큼 반복 (현재 위치가 어느 세그먼트 위에 있는지 찾기)
        {
            tempParam += Vector3.Distance(ps.a, ps.b); // 선분의 길이를 tempParam에 누적
            if (lastParam <= tempParam) 
            {
                currentSegment = ps;
                break;
            }
        }

        if (currentSegment == null) return 0f;
        
        // 주어진 현재 위치를 통해 어느 방향으로 가야할지 결정
        Vector3 currPos = position - currentSegment.a;
        Vector3 segmentDirection = currentSegment.b - currentSegment.a; // 선분의 방향 벡터를 계산
        segmentDirection.Normalize(); // 방향만 유지
        
        // 벡터 투영을 통해서 세그먼트 내의 포인트를 찾는다.
        Vector3 pointInSegment = Vector3.Project(currPos, segmentDirection);
        
        // GetParam 함수에서 경로 중 다음 위치를 반환한다.
        param = tempParam - Vector3.Distance(currentSegment.a, currentSegment.b); // 누적 거리에서 해당 세그먼트 이전 길이 빼고
        param += pointInSegment.magnitude; // 현재 세그먼트 안에서 거리
        return param;
    }

    public Vector3 GetPosition(float param) // 주어진 param(길이위치)을 실제 공간 좌표로 변환
    {
        // 경로 사이에 존재하는 주어진 현재 위치를 통해, 상응하는 세그먼트를 찾는다.
        Vector3 position = Vector3.zero;
        PathSegment currentSegment = null;
        float tempParam = 0f;
        foreach (PathSegment ps in segments) 
        {
            tempParam += Vector3.Distance(ps.a, ps.b);
            if (param <= tempParam) // 누적거리와 param을 비교하여
            {
                currentSegment = ps; // param이 해당되는 선분 찾기
                break;
            }
        }
        if (currentSegment == null) return Vector3.zero;
        
        // GetPosition 함수에서 파라미터를 공간상의 위치로 변환 후 반환
        Vector3 segmentDirection = currentSegment.b - currentSegment.a; // 시작점에서 방향벡터 * 거리 를 더하면 최종위치
        segmentDirection.Normalize();
        tempParam -= Vector3.Distance(currentSegment.a, currentSegment.b);
        tempParam = param - tempParam;
        position = currentSegment.a + segmentDirection * tempParam;
        return position;
    }
}
