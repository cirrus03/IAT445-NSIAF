using System.Collections;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }

    [Header("Damage Flash (optional)")]
    [SerializeField] private bool flashOnDamage = true;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Invincibility (Player)")]
    [SerializeField] private float invincibilityTime = 0.6f;
    public bool IsInvincible { get; private set; }
    private Coroutine invincibleRoutine;

    [Header("UI Stuff")]
    public HealthBar healthBar;

    public static event Action PlayerDeath;

    // If you assign this, it will temporarily swap to that sprite instead
    // If you leave it empty, it will just tint red instead.
    [SerializeField] private Sprite hurtSprite;

    private SpriteRenderer sr;
    private Color originalColor;
    private Sprite originalSprite;
    private Coroutine flashRoutine;

    bool isDying = false;

    private void Awake()
    {
        currentHealth = startingHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            originalSprite = sr.sprite;
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(startingHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDying) return; //shouldnt be taking extra damage if youre dead (plan B)
        if (IsInvincible) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (flashOnDamage && sr != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} died");

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }

            // Disable ALL gameplay scripts on player
            foreach (var script in GetComponents<MonoBehaviour>())
            {
                if (script != this) script.enabled = false;
            }

            PlayerDeath?.Invoke(); // UI / death screen later
        }
    }

    public void StartInvincibility(float seconds = -1f)
    {
        if (seconds <= 0f) seconds = invincibilityTime;

        if (invincibleRoutine != null) StopCoroutine(invincibleRoutine);
        invincibleRoutine = StartCoroutine(InvincibleRoutine(seconds));
    }

    IEnumerator InvincibleRoutine(float seconds)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(seconds);
        IsInvincible = false;
        invincibleRoutine = null;
    }

    IEnumerator FlashDamage()
    {
        if (sr == null) yield break;

        if (hurtSprite != null) sr.sprite = hurtSprite;
        else sr.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        // restores only when not dead
        if (!isDying)
        {
            if (hurtSprite != null) sr.sprite = originalSprite;
            else sr.color = originalColor;
        }

        flashRoutine = null;
    }
}
