using UnityEngine;

public class EnemyResettable : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    private EnemyHealth enemyHealth;
    private Rigidbody2D rb;
    private Collider2D[] allColliders;
    private SpriteRenderer sr;
    private Animator anim;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        allColliders = GetComponentsInChildren<Collider2D>(true);
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void ResetEnemy()
    {
        gameObject.SetActive(true);

        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if (rb.bodyType == RigidbodyType2D.Kinematic)
                rb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (enemyHealth != null)
        {
            enemyHealth.ResetEnemyState();
        }

        if (anim != null)
        {
            anim.enabled = true;
            anim.Rebind();
            anim.Update(0f);
        }

        if (sr != null)
        {
            sr.enabled = true;
        }

        foreach (var col in allColliders)
        {
            if (col != null)
                col.enabled = true;
        }

        var enemyDamage = GetComponentInChildren<EnemyDamage>(true);
        if (enemyDamage != null)
            enemyDamage.enabled = true;

        var flying = GetComponent<EnemyFlying>();
        if (flying != null)
            flying.enabled = true;

    }
}