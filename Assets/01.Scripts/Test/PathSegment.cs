using UnityEngine;

public class PathSegment
{
    public Vector3 a; // 시작점 변수
    public Vector3 b; // 도착점 변수
    
    public PathSegment() : this(Vector3.zero, Vector3.zero) { } // 기본 생성자로 제로화

    // 시작점과 끝점을 받아와 저장시키는 메서드
    public PathSegment(Vector3 a, Vector3 b)
    {
        this.a = a;
        this.b = b;
    }
}
