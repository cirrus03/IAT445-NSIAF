using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTimeAtPoints = 0f;

    private Rigidbody2D rb;
    private Vector2 currentTarget;
    private float waitTimer = 0f;

    public Vector2 DeltaMovement { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("MovingPlatform missing pointA or pointB.");
            enabled = false;
            return;
        }

        rb.position = pointA.position;
        currentTarget = pointB.position;
    }

    private void FixedUpdate()
    {
        DeltaMovement = Vector2.zero;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 nextPosition = Vector2.MoveTowards(currentPosition, currentTarget, moveSpeed * Time.fixedDeltaTime);

        DeltaMovement = nextPosition - currentPosition;
        rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, currentTarget) < 0.01f)
        {
            currentTarget = currentTarget == (Vector2)pointA.position
                ? (Vector2)pointB.position
                : (Vector2)pointA.position;

            waitTimer = waitTimeAtPoints;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(pointA.position, 0.1f);
        Gizmos.DrawWireSphere(pointB.position, 0.1f);
    }
}