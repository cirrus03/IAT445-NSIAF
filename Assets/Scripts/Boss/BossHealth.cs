using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    [Header("State")]
    public bool IsInvincible { get; private set; } = false;

    private bool isDead = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        if (IsInvincible) return;

        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        Debug.Log($"Boss took {damageAmount} damage. HP: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
        Debug.Log("Boss invincible: " + value);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("BOSS DEAD");

        // disable behavior / movement
        var agent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        if (agent != null) agent.enabled = false;

        // disable colliders
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // TODO:
        // - play animation
        // - trigger win condition
    }
}