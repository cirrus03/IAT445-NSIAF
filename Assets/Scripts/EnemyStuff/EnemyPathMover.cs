using UnityEngine;

public class EnemyPathMover : MonoBehaviour
{
    [Header("Path Points")]
    [SerializeField] private Transform[] pathPoints;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.05f;
    [SerializeField] private bool loopPath = true;
    [SerializeField] private bool pingPong = false;

    private Rigidbody2D rb;
    private int currentPointIndex = 0;
    private int direction = 1;

    private bool isFacingRight = true;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (PauseMenu.isPaused) return;

        if (pathPoints == null || pathPoints.Length == 0 || rb == null)
            return;

        Transform targetPoint = pathPoints[currentPointIndex];
        if (targetPoint == null) return;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = targetPoint.position;

        Vector2 nextPos = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.fixedDeltaTime);
        Vector2 moveDir = targetPos - currentPos;

        rb.MovePosition(nextPos);

        UpdateFacing(moveDir);

        if (Vector2.Distance(nextPos, targetPos) <= reachDistance)
        {
            AdvanceToNextPoint();
        }
    }

    private void AdvanceToNextPoint()
    {
        if (pathPoints.Length <= 1)
            return;

        if (pingPong)
        {
            if (currentPointIndex == pathPoints.Length - 1)
                direction = -1;
            else if (currentPointIndex == 0)
                direction = 1;

            currentPointIndex += direction;
            return;
        }

        currentPointIndex++;

        if (currentPointIndex >= pathPoints.Length)
        {
            if (loopPath)
                currentPointIndex = 0;
            else
                currentPointIndex = pathPoints.Length - 1;
        }
    }

    private void UpdateFacing(Vector2 moveDir)
    {
        if (moveDir.x > 0.01f && !isFacingRight)
        {
            Flip();
        }
        else if (moveDir.x < -0.01f && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public void StopMovingForever()
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathPoints == null || pathPoints.Length == 0)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            if (pathPoints[i] == null) continue;

            Gizmos.DrawWireSphere(pathPoints[i].position, 0.1f);

            if (i < pathPoints.Length - 1 && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }

        if (loopPath && !pingPong && pathPoints.Length > 1 &&
            pathPoints[0] != null && pathPoints[pathPoints.Length - 1] != null)
        {
            Gizmos.DrawLine(pathPoints[pathPoints.Length - 1].position, pathPoints[0].position);
        }
    }
}