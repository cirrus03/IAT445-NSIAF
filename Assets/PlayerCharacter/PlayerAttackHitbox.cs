using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 1f;

    [Header("Enemy Knockback")]
    public float knockbackX = 6f;
    public float knockbackY = 2f;

    Transform playerRoot;

    void Awake()
    {
        playerRoot = transform.root;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // enemies only
        if (other.CompareTag("Player")) return;

        Health hp = other.GetComponent<Health>();
        if (hp == null) return;

        hp.TakeDamage(damage);

        Rigidbody2D enemyRb = other.attachedRigidbody;
        if (enemyRb != null)
        {
            // enemy pushed away from player
            float dir = Mathf.Sign(other.transform.position.x - playerRoot.position.x);
            if (dir == 0) dir = 1f;

            enemyRb.linearVelocity = Vector2.zero; // reset current motion
            enemyRb.AddForce(new Vector2(dir * knockbackX, knockbackY), ForceMode2D.Impulse);
        }
    }
}
