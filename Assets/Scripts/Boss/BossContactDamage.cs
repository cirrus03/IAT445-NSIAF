using UnityEngine;

public class BossContactDamage : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 1f;

    [Header("Knockback")]
    public float knockbackX = 12f;
    public float knockbackY = 6f;

    [Header("Timing")]
    public float hitCooldown = 0.2f;

    private float timer;

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (timer > 0f) return;
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (playerHealth.IsInvincible) return;

        // Damage
        playerHealth.TakeDamage(damage);
        timer = hitCooldown;

        // Knockback
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (dir == 0) dir = 1f;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * knockbackX, knockbackY), ForceMode2D.Impulse);
        }

        // Stun / hitstop (reuses your existing system)
        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.StartCoroutine(pm.DamageStunRoutine(0.18f));
            pm.StartCoroutine(pm.HitStopRoutine(0.05f));
        }
    }
}