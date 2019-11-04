using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int startingHealth = 5;
    private Animator animator;
    private int currentHealth;

    public event Action<float> OnHit = delegate { };
    public event Action OnDeath = delegate { };

    private void OnEnable()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        OnHit(damageAmount);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath();
    }

    /// <summary>
    /// Adds an amount to the current health
    /// </summary>
    /// <param name="amount"></param>
    public void AddHealth(int amount)
    {
        currentHealth = Math.Min(startingHealth, currentHealth + amount);
    }

}
