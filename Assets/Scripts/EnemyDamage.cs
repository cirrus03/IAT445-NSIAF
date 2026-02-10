using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damage = 1f;
    public float knockback = 1f;
    //iframes
    float lastHitTime;
    public float hitCooldown = 0.5f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < lastHitTime + hitCooldown) 
        {
            return;
        }
        lastHitTime = Time.time;

        if (!other.CompareTag("Player")) 
        {
        return;
        }

        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth == null) 
        {
        return;
        }

        playerHealth.TakeDamage(damage);

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(other.transform.position.x - transform.root.position.x);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(dir * knockback, 4f), ForceMode2D.Impulse);
        }
    }
}
