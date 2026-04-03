using UnityEngine;

public class ProtectionBubble : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushForce = 10f;
    public bool affectFlyingEnemies = true;
    public bool affectJumpingEnemies = true;

    private Transform playerRoot;

    private void Awake()
    {
        playerRoot = transform.root;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Rigidbody2D enemyRb = other.attachedRigidbody;
        if (enemyRb == null)
            return;

        Vector2 pushDir = (other.transform.position - playerRoot.position).normalized;

        if (pushDir.sqrMagnitude < 0.001f)
        {
            pushDir = Vector2.right;
        }

        enemyRb.linearVelocity = new Vector2(pushDir.x * pushForce, enemyRb.linearVelocity.y);
    }
}