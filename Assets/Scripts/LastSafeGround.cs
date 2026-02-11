using UnityEngine;

public class LastSafeGround : MonoBehaviour
{
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    public Vector2 respawnOffset = new Vector2(0f, 0.8f);

    private Rigidbody2D rb;
    public Vector2 LastSafePos { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        LastSafePos = rb.position;
    }

    void FixedUpdate()
    {
        bool grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);

        if (grounded && rb.linearVelocity.y <= 0.1f)
            LastSafePos = rb.position;
    }

    public void RespawnToLastSafe()
    {
        rb.position = LastSafePos + respawnOffset;
        rb.linearVelocity = Vector2.zero;
    }
}
