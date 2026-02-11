using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool respawnPlayerOnHit = true;
    [SerializeField] private float invincibilityAfterHit = 0.6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Health health = collision.GetComponent<Health>();
        if (health == null) return;

        // iframes
        if (health.IsInvincible) return;

        health.TakeDamage(damage);

        if (respawnPlayerOnHit)
        {
            collision.GetComponent<LastSafeGround>()?.RespawnToLastSafe();
            health.StartInvincibility(invincibilityAfterHit);
        }
    }
}
