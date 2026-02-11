using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth; 
    public float currentHealth {get; private set; }
    public HealthBar healthBar;
    public static event Action PlayerDeath;
    private SpriteRenderer sr;
    private Color originalColor;
    private Sprite originalSprite;

    private void Awake()
    {
        currentHealth = startingHealth;

        // works even if the SpriteRenderer is on a child
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            originalSprite = sr.sprite;
        }
        healthBar.SetMaxHealth(startingHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        if (IsInvincible) return;
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
            //respawn / player death, game over screen, etc
        }
    }

