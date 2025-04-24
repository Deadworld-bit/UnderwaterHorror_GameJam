using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        Debug.Log($"Current Health: {currentHealth} / {maxHealth}");
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}