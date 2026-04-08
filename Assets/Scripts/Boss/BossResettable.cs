using UnityEngine;

public class BossResettable : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    private BossHealth bossHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        bossHealth = GetComponent<BossHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetBoss()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (bossHealth != null)
            bossHealth.ResetBoss();
    }
}