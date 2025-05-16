using System;
using UnityEngine;

public class AttackSetting : MonoBehaviour
{
     private Animator animator;
    [SerializeField] private AttackData[] attacks; // 애니메이션에 대응되는 공격 데이터
     private Combat combat;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        combat = GetComponent<Combat>();
    }

    public void Attack(int index)
    {
        if (index >= 0 && index < attacks.Length)
        {
            combat.SetAttack(attacks[index]);
            animator.SetTrigger("Attack" + index); // ex) Attack0, Attack1 등
        }
    }
}
