using UnityEngine;

public class HealthBob : MonoBehaviour
{
    public float bobHeight = 0.15f;
    public float bobSpeed = 2f;
    public float rotationSpeed = 0f;

    private Vector3 startPos;
    private bool landed = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!landed) return;

        float newY = (Mathf.Sin(Time.time * bobSpeed) + 1f) * 0.5f * bobHeight;
        transform.position = startPos + new Vector3(0f, newY, 0f);

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (landed) return;

        if (collision.collider.CompareTag("Ground"))
        {
            landed = true;

            if (rb != null)
                rb.bodyType = RigidbodyType2D.Kinematic;

            startPos = transform.position;
        }
    }
}