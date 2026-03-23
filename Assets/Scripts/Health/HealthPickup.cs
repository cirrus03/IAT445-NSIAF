using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f;
    [SerializeField] private bool destroyOnPickup = true;

    private SoundFXManager audioManager;

    private Collider2D solidCollider;
    private Rigidbody2D rb;
    private bool landed = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundFXManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        solidCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        audioManager.PlaySFX(audioManager.pickUpHeal);

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // if (playerHealth.currentHealth >= playerHealth.MaxHealth) return;

        playerHealth.Heal(healAmount);

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}