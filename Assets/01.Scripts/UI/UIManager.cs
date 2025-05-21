using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get => instance;
        set => instance = value;
    }
    public Health playerHealth;
    
    public float maxHealth;
    public float currentHealth;
    
    public Slider HealthBar;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (playerHealth)
        {
            maxHealth = playerHealth.MaxHealth;
            playerHealth.OnHealthChanged += SetHealth;
            SetHealth(playerHealth.CurrentHealth); // 시작 시 체력 표시
        }
    }
    public void SetHealth(float current)
    {
        currentHealth = current;
        HealthBar.value = current / maxHealth;
    }
}
