using UnityEngine;

public class EnemyAttackState : EnemyState
{
    // 공격 대상으로 삼을 대상의 Transform 정보를 저장할 변수
    private GameObject attackTarget; 

    // 생성자: EnemyController와 함께 공격 대상 콜라이더를 매개변수로 받음
    public EnemyAttackState(EnemyController enemy) : base(enemy) {}

    public override void Enter()
    {
        enemy.Agent.isStopped = true;
        enemy.attack.Attack(0);
    }
    
}
