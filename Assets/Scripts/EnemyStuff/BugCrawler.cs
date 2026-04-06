using UnityEngine;

public class BugCrawler : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints; // TopLeft, TopRight, BottomRight, BottomLeft in order

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 180f;
    public float waypointReachedDistance = 0.15f;

    [Header("Direction")]
    public bool reverseDirection = false;

    [Header("Sprite")]
    public bool flipSprite = true;

    private Rigidbody2D rb;
    private int currentWaypointIndex = 0;
    private Vector2 currentTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        currentTarget = waypoints[currentWaypointIndex].position;
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        MoveTowardWaypoint();
        CheckWaypointReached();
    }

    void MoveTowardWaypoint()
    {
        Vector2 toTarget = ((Vector2)waypoints[currentWaypointIndex].position - rb.position);
        Vector2 moveDir = toTarget.normalized;
        float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 180f;
        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        transform.localScale = new Vector3(flipSprite ? -1f : 1f, 1f, 1f);
    }

    void CheckWaypointReached()
    {
        float dist = Vector2.Distance(rb.position, waypoints[currentWaypointIndex].position);
        if (dist <= waypointReachedDistance)
        {
            if (reverseDirection)
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                    currentWaypointIndex = waypoints.Length - 1;
            }
            else
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                    currentWaypointIndex = 0;
            }
        }
    }

    public void FlipDirection()
    {
        reverseDirection = !reverseDirection;
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.1f);

            int next = (i + 1) % waypoints.Length;
            if (waypoints[next] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
        }

        if (Application.isPlaying && waypoints[currentWaypointIndex] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(waypoints[currentWaypointIndex].position, 0.15f);
        }
    }
}