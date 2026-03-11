using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool respawnPlayerOnHit = true;
    [SerializeField] private float invincibilityAfterHit = 0.6f;
    [SerializeField] private bool ignoreIFrames = true;
    [SerializeField] private float respawnLockTime = 0.4f;
    [SerializeField] private float respawnBounceY = 6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHitPlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryHitPlayer(collision);
    }

    private void TryHitPlayer(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (!ignoreIFrames && playerHealth.IsInvincible) return;

        if (!playerHealth.IsInvincible || ignoreIFrames)
        {
            playerHealth.TakeDamage(damage);
        }

        if (respawnPlayerOnHit)
        {
            var pm = collision.GetComponent<PlayerMovement>();
            var rb = collision.GetComponent<Rigidbody2D>();
            var respawner = collision.GetComponent<LastSafeGround>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            respawner?.RespawnToLastSafe();

            playerHealth.StartInvincibility(invincibilityAfterHit);

            if (pm != null)
            {
                pm.StartCoroutine(pm.HazardRespawnRoutine(respawnLockTime, respawnBounceY));
            }
        }
    }
}