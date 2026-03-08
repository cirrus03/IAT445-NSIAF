using UnityEngine;

public class LastSafeGround : MonoBehaviour
{
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Respawn")]
    public float respawnHeight = 1.0f;
    public float raycastDistance = 2f;

    private Rigidbody2D rb;
    private bool wasGrounded;

    public Vector2 LastSafePos { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        LastSafePos = rb.position;
    }

    void FixedUpdate()
    {
        bool grounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);

        // Save only when landing
        if (grounded && !wasGrounded)
        {
            SaveSafePosition();
        }

        wasGrounded = grounded;
    }

    void SaveSafePosition()
    {
        Vector2 origin = new Vector2(transform.position.x, groundCheckPos.position.y + 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            // Save a point directly above the top surface
            LastSafePos = new Vector2(transform.position.x, hit.point.y + respawnHeight);
        }
    }

    public void RespawnToLastSafe()
    {
        rb.position = LastSafePos;
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

            Gizmos.color = Color.yellow;
            Vector2 origin = new Vector2(transform.position.x, groundCheckPos.position.y + 0.5f);
            Gizmos.DrawLine(origin, origin + Vector2.down * raycastDistance);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(LastSafePos, 0.08f);
    }
}