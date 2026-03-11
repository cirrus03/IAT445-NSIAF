using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f;
    [SerializeField] private bool destroyOnPickup = true;

    private Collider2D solidCollider;
    private Rigidbody2D rb;
    private bool landed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        solidCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // if (playerHealth.currentHealth >= playerHealth.MaxHealth) return;

        playerHealth.Heal(healAmount);

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}