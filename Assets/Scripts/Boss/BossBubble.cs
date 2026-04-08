using UnityEngine;

public class BossBubble : MonoBehaviour
{
    [Header("Bounce Force")]
    public float bounceX = 14f;
    public float bounceY = 6f;

    [Header("Cooldown")]
    public float bounceCooldown = 0.2f;

    private float timer = 0f;

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Shield Enter with: " + other.name);
        TryBounce(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Shield Stay with: " + other.name);
        TryBounce(other);
    }

    private void TryBounce(Collider2D other)
    {
        if (timer > 0f) return;

        GameObject playerObj = other.CompareTag("Player")
            ? other.gameObject
            : other.GetComponentInParent<PlayerHealth>()?.gameObject;

        if (playerObj == null)
        {
            Debug.Log("No player found from collider: " + other.name);
            return;
        }

        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.Log("No Rigidbody2D found on player");
            return;
        }

        float dir = Mathf.Sign(playerObj.transform.position.x - transform.position.x);
        if (dir == 0) dir = 1f;

        Debug.Log("Bouncing player: " + playerObj.name);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * bounceX, bounceY), ForceMode2D.Impulse);

        var pm = playerObj.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.StartCoroutine(pm.DamageStunRoutine(0.1f));
            pm.StartCoroutine(pm.HitStopRoutine(0.03f));
        }

        timer = bounceCooldown;
    }
}