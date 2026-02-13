using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool respawnPlayerOnHit = true;
    [SerializeField] private float invincibilityAfterHit = 0.6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // iframes
        if (playerHealth.IsInvincible) return;

        playerHealth.TakeDamage(damage);

        if (respawnPlayerOnHit)
        {
            collision.GetComponent<LastSafeGround>()?.RespawnToLastSafe();
            playerHealth.StartInvincibility(invincibilityAfterHit);
        }
    }
}
