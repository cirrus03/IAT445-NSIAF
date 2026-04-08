using System.Collections;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damage = 1f;

    [Header("Knockback")]
    public float knockbackX = 12f;
    public float knockbackY = 6f;

    [Header("Frezze frame + Stun")]
    public float hitStopTime = 0.05f;   // freeze frame feel
    public float stunTime = 0.18f;      // controls locked

    [Header("Contact Re-hit")]
    public float touchDamageCooldown = 0.15f;

    private float touchDamageTimer = 0f;

    private void Update()
    {
        if (touchDamageTimer > 0f)
        {
            touchDamageTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("ontrigger enter2d going to try and damage player");
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("ontrigger stay 2d going to try and damage player");
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (touchDamageTimer > 0f) return;

        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // iframes gate (prevents multi-hits)
        if (playerHealth.IsInvincible) return;

        // Deal damage to player
        playerHealth.TakeDamage(damage);

        // local cooldown so it doesnt hit every frame while overlapping
        touchDamageTimer = touchDamageCooldown;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (dir == 0) dir = 1f;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * knockbackX, knockbackY), ForceMode2D.Impulse);
        }

        // Stun + hit stop
        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.StartCoroutine(pm.DamageStunRoutine(stunTime));
            pm.StartCoroutine(pm.HitStopRoutine(hitStopTime));
        }

        EnemyFlying flying = GetComponentInParent<EnemyFlying>();
        if (flying != null)
        {
            flying.NotifyHitPlayer();
        }
    }
}