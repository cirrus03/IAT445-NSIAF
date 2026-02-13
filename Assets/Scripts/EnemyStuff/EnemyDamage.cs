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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // iframes gate (prevents multi-hits)
        if (playerHealth.IsInvincible) return;

        // Deal damage to player
        playerHealth.TakeDamage(damage);

        // Knockback (use direction away from enemy root)
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(other.transform.position.x - transform.root.position.x);
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
