using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Aggro")]
    public float aggroRange = 6f;      // start chasing
    public float deaggroRange = 8f;    // stop chasing 
    public float loseSightDelay = 2.5f;   // how long it remembers you after losing sight
    private float loseSightTimer = 0f;
    private bool isAggro;

    [Header("Search")]
    public float searchSpeed = 2.5f;
    public float reachLastSeenDistance = 0.4f;

    private Vector2 lastSeenPos;
    private bool hasLastSeen;

    [Header("Movement")]
    public float flySpeed = 3f;
    public float accel = 12f;
    public float stopDistance = 0.5f;

    [Header("Patrol")]
    public float patrolSpeed = 1.5f;
    public float patrolDistance = 3f;     // how far left/right from start
    public float patrolHeightBob = 0.5f;  // small up/down bob
    public float bobSpeed = 2f;
    private Vector2 startPos;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleMask;        // ground/ wall checks
    public float avoidCheckDistance = 0.6f;
    public float avoidRadius = 0.25f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    bool isFacingRight = false;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        startPos = rb.position;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        // checking for aggro
        float distToPlayer = Vector2.Distance(rb.position, player.position);
        bool canSeePlayer = HasLineOfSightToPlayer();
        
        if (!isAggro)
        {
            if (distToPlayer <= aggroRange && canSeePlayer)
            {
                isAggro = true;
                loseSightTimer = 0f;
            }
        }
        else
        {
            if (canSeePlayer)
            {
                loseSightTimer = 0f; // reset memory when it sees you again
                lastSeenPos = player.position;
                hasLastSeen = true;
            }
            else
            {
                loseSightTimer += Time.fixedDeltaTime;
            }

            if (distToPlayer >= deaggroRange || loseSightTimer >= loseSightDelay)
            {
                isAggro = false;
                loseSightTimer = 0f;
            }
        }

        // velocoty
        Vector2 desiredVel;

        if (isAggro)
        {
            if (canSeePlayer)
            {
                desiredVel = ChasePlayer(distToPlayer);
            }
            else if (hasLastSeen)
            {
                Vector2 toLast = lastSeenPos - rb.position;

                // if reach the last seen point, let deaggro timer handle giving up (idk what other way to handle this)
                if (toLast.magnitude <= reachLastSeenDistance)
                    desiredVel = Vector2.zero;
                else
                    desiredVel = toLast.normalized * searchSpeed;
            }
            else
            {
                desiredVel = Vector2.zero;
            }
        }
        else
        {
            desiredVel = PatrolVelocity();
        }

        // obstacle avoid
        desiredVel = AvoidObstacles(desiredVel);

        // steering
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVel, accel * Time.fixedDeltaTime);
        // Flip based on movement direction (same as player)
        if (rb.linearVelocity.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (rb.linearVelocity.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    Vector2 ChasePlayer(float distToPlayer)
    {
        Vector2 toPlayer = (Vector2)player.position - rb.position;

        if (distToPlayer <= stopDistance)
            return Vector2.zero;

        return toPlayer.normalized * flySpeed;
    }

    bool HasLineOfSightToPlayer()
    {
        if (!player) return false;

        Vector2 origin = rb.position;
        Vector2 toPlayer = (Vector2)player.position - origin;
        float dist = toPlayer.magnitude;

        // raycast obstacles, If it hits something; line of sight blocked
        RaycastHit2D hit = Physics2D.Raycast(origin, toPlayer.normalized, dist, obstacleMask);

        return hit.collider == null;
    }


    void Patrol()
    {
        Vector2 desiredVel = PatrolVelocity();
        desiredVel = AvoidObstacles(desiredVel);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVel, accel * Time.fixedDeltaTime);
    }

    Vector2 PatrolVelocity()
    {
        // back and fourth movement
        float xOffset = Mathf.PingPong(Time.time * patrolSpeed, patrolDistance * 2f) - patrolDistance;
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * patrolHeightBob;

        Vector2 patrolTarget = startPos + new Vector2(xOffset, yOffset);
        Vector2 toTarget = patrolTarget - rb.position;

        // when close, slow down
        if (toTarget.magnitude < 0.2f) return Vector2.zero;

        return toTarget.normalized * patrolSpeed;
    }

    Vector2 AvoidObstacles(Vector2 desiredVel)
{
    if (player)
    {
        float d = Vector2.Distance(rb.position, player.position);
        if (d < 1f && HasLineOfSightToPlayer())
            return desiredVel;
    }

    if (desiredVel == Vector2.zero) {
        return desiredVel;
    }

    Vector2 dir = desiredVel.normalized;
    float speed = desiredVel.magnitude;

    float checkDist = Mathf.Max(avoidCheckDistance, rb.linearVelocity.magnitude * 0.25f);

    // movment straight
    if (!Physics2D.CircleCast(rb.position, avoidRadius, dir, checkDist, obstacleMask))
        {
            return desiredVel;
        }


    // try to wrap around
    Vector2[] options = new Vector2[]
    {
        (dir + Vector2.up).normalized,
        (dir + Vector2.down).normalized,
        Vector2.up,
        Vector2.down,
        (dir + Vector2.left).normalized,
        (dir + Vector2.right).normalized,
        Vector2.left,
        Vector2.right
    };

    foreach (var opt in options)
    {
        if (!Physics2D.CircleCast(rb.position, avoidRadius, opt, checkDist, obstacleMask))
            return opt * speed;
    }

    // if nothing, stops and leaves
    return Vector2.zero;
}


    // aggro range 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deaggroRange);
    }
}
