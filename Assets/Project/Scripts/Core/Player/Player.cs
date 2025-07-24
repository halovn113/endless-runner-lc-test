using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    public float damage = 20f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        EventManager.PlayerHealthChanged(currentHealth);
        EventManager.OnGameStart += ResetHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        EventManager.PlayerHealthChanged(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        EventManager.PlayerHealthChanged(currentHealth);
    }

    private void Die()
    {
        EventManager.PlayerDied();
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        EventManager.PlayerHealthChanged(currentHealth);
    }

}
