
using UnityEngine;

public class PathFollower : Seek
{
    public Path path;
    public float pathOffset = 0.0f;
    private float currentParam;

    public override void Awake() // 대상을 지정
    {
        base.Awake();
        target = new GameObject();
        currentParam = 0f;
    }

    public override Steering GetSteering() 
    {
        // 목표위치를 지정하고 Seek클래스를 적용하기 위해서 Path 클래스의 추상화에 의존한다.
        currentParam = path.GetParam(transform.position, currentParam);
        float targetParam = currentParam + pathOffset;
        target.transform.position = path.GetPosition(targetParam);
        return base.GetSteering();
    }
}
