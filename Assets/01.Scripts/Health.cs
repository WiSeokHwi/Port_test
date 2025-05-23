using JetBrains.Annotations;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public System.Action OnDie;
    public System.Action<float> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, [CanBeNull] GameObject attacker = null)
    {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log("데미지 : " + amount + " 공격자 : " + attacker);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDie?.Invoke();
        // 죽는 연출, 애니메이션, 이펙트 등은 이벤트 구독을 통해 외부에서 처리
        // ex) animator.SetTrigger("Die");
    }
}
