using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject
{
    public float damage = 10f; // 데미지
    public float range = 2f; // 공격 길이
    public float angle = 90f; // 공격 범위 각도
    public Vector3 offset = Vector3.forward; // 공격 방향
    public LayerMask targetMask; // 감지할 타겟 레이어
}