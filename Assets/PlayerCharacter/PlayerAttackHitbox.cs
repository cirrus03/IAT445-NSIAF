using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 1f;

    [Header("Enemy Knockback")]
    public float knockbackX = 13f;
    public float knockbackY = 3f;

    [Header("Pogo")]
    public bool pogoOnHit = true;
    public float pogoBounceY = 14f;
    public bool pogoOnlyWhenFalling = false;
    public string hazardTag = "Hazard";

    Transform playerRoot;
    Collider2D hitboxCol;
    Rigidbody2D playerRb;
    bool smacked = false;

    void Awake()
    {
        playerRoot = transform.root;
        playerRb = playerRoot.GetComponent<Rigidbody2D>();
        hitboxCol = GetComponent<Collider2D>();
    }
    void OnEnable()
    {
        smacked = false;
        if (hitboxCol != null)
        {
            hitboxCol.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (smacked)
        {
            return;
        }
        // enemies only
        if (other.CompareTag("Player")) return;

        bool holyShootIJustHitAPogo = false;

        Health hp = other.GetComponent<Health>();

        if (hp != null)
        {
            holyShootIJustHitAPogo = true;

            ApplyPooogo();

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

        if (other.CompareTag(hazardTag)) //pogo off hazard
        {
            holyShootIJustHitAPogo = true;
            ApplyPooogo();
        }

        if (pogoOnHit && holyShootIJustHitAPogo && playerRb != null)
        {
            if (!pogoOnlyWhenFalling || playerRb.linearVelocity.y <= 0f)
            {
                // give a clean upward bounce
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, pogoBounceY);
            }
        }
            if (holyShootIJustHitAPogo)
        {
            smacked = true;
            if (hitboxCol != null) hitboxCol.enabled = false;
        }
    }
        private void ApplyPooogo()
    {
        if (!pogoOnHit || playerRb == null) return;
        if (pogoOnlyWhenFalling && playerRb.linearVelocity.y > 0f) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, pogoBounceY);
    }

}
