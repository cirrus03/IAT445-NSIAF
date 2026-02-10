using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth; 
    public float currentHealth {get; private set; }
    public HealthBar healthBar;
    public static event Action PlayerDeath;

    private void Awake()
    {
        currentHealth = startingHealth;
        healthBar.SetMaxHealth(startingHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);
        Debug.Log($"Took {damageAmount} damage. Current health: {currentHealth}");
        if (healthBar != null) 
            healthBar.SetHealth(currentHealth); 
            if (currentHealth > 0) 
            { 
                Debug.Log("Still Alive"); 
            } 
            else 
            { 
                Debug.Log("You Died");
                PlayerDeath?.Invoke();
            }
    }
}
