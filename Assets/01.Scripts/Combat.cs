using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint; // 판정 기준 위치 (예: 캐릭터 앞)
    private AttackData currentAttack; // 어떤 공격인지 담을 변수
    public Weapon currentWeapon;
    

    public void SetAttack(AttackData attackData) // 공격을 변경하는 메서드
    {
        currentAttack = currentWeapon.attackData;
    }

    // 애니메이터 이벤트에서 호출
    public void OnAttackHitCheck()
    {
        if (currentAttack == null) return; // 공격이 비어있다면 리턴

        Vector3 center = transform.position + transform.rotation * currentAttack.offset; // 공격이 나갈 지점과 방향 설정
        
        // 구 콜라이더 생성, (공격중심, 공격 범위, 감지할 레이어)
        Collider[] hitTargets = Physics.OverlapSphere(center, currentAttack.range, currentAttack.targetMask);

        foreach (Collider col in hitTargets) // 생성된 공격 범위 콜라이더에 감지된 콜라이더를 다 검사
        {
            // 각도 체크
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized; // 타겟과 나의 방향 구하기
            float angle = Vector3.Angle(transform.forward, dirToTarget); // 내 전방 기준으로 타겟방향의 각도 저장
            
            float distance = Vector3.Distance(col.transform.position, transform.position);
            
            if (angle <= currentAttack.angle * 0.5f && distance <= currentAttack.range) // 타겟 방향이 currentAttack.angle 안에 있다면 ( 양쪽으로 검사하기때문에 90도라면 -45~45 )
            {
                if (col.TryGetComponent(out IDamageable target)) // 감지된 콜라이더에 IDamageable이 있다면 그 IDamageable을 target으로 지정
                {
                    
                    target.TakeDamage(currentWeapon.damage, gameObject);// 타겟의 TakeDamage(공격 데이미, 내 게임 오브젝트) 발동
                }
            }
        }

        Debug.DrawRay(center, transform.forward * currentAttack.range, Color.red, 1f);
    }
}
