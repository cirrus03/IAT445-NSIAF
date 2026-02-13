using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    public bool enabledKnockback = true;
    public float knockbackX = 10f;
    public float knockbackY = 2f;
    public bool resetVelocity = true;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Apply(Vector2 attackerPos)
    {
        if (!enabledKnockback || rb == null) return;

        float dir = Mathf.Sign(((Vector2)transform.position - attackerPos).x);
        if (dir == 0) dir = 1f;

        if (resetVelocity) rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * knockbackX, knockbackY), ForceMode2D.Impulse);
    }
}
