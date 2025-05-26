using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    Slider healthBar;
    private Health health;
    float currentHealth;
    

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
        health = GetComponentInParent<Health>();
        
    }

    private void Start()
    {
        currentHealth = health.CurrentHealth;
        health.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(currentHealth);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    void UpdateHealthBar(float currentHealth) 
    {
        this.currentHealth = currentHealth;
        healthBar.value = currentHealth / health.MaxHealth;
    }
    
    
}
