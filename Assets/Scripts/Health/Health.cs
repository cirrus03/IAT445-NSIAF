using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }

    [Header("Damage Flash (optional)")]
    [SerializeField] private bool flashOnDamage = true;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Invincibility (Player)")]
    [SerializeField] private float invincibilityTime = 0.6f;
    public bool IsInvincible { get; private set; }
    private Coroutine invincibleRoutine;


    // If you assign this, it will temporarily swap to that sprite instead
    // If you leave it empty, it will just tint red instead.
    [SerializeField] private Sprite hurtSprite;

    private SpriteRenderer sr;
    private Color originalColor;
    private Sprite originalSprite;
    private Coroutine flashRoutine;

    bool isDying = false;


    private void Awake()
    {
        currentHealth = startingHealth;

        // works even if the SpriteRenderer is on a child
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            originalSprite = sr.sprite;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (IsInvincible) return;
        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);

        if (flashOnDamage && sr != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} died");

            if (!CompareTag("Player"))
            {
                isDying = true;
                StartCoroutine(DieRoutine());
            }
            //respawn / player death, game over screen, etc
        }
    }

    IEnumerator DieRoutine()
    {
        // stop all movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // freezes physics movement
        }

        // disable enemy AI scripts (if theyre there)
        var flying = GetComponent<EnemyFlying>();
        if (flying != null) flying.enabled = false;

        var jumper = GetComponent<EnemyJumper>();
        if (jumper != null) jumper.enabled = false;


        // disable damage hitbox script so it can't hurt player while dying
        var dmg = GetComponentInChildren<EnemyDamage>();
        if (dmg != null) dmg.enabled = false;

        // show death flash
        if (sr != null)
        {
            if (hurtSprite != null) sr.sprite = hurtSprite;
            else sr.color = Color.red;
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
public void StartInvincibility(float seconds = -1f)
    {
        if (seconds <= 0f) seconds = invincibilityTime;

        if (invincibleRoutine != null) StopCoroutine(invincibleRoutine);
        invincibleRoutine = StartCoroutine(InvincibleRoutine(seconds));
    }

    IEnumerator InvincibleRoutine(float seconds)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(seconds);
        IsInvincible = false;
        invincibleRoutine = null;
    }



    IEnumerator FlashDamage()
    {
        if (sr == null) yield break;

        if (hurtSprite != null)
            sr.sprite = hurtSprite;
        else
            sr.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        // restores only when not dead
        if (!isDying)
        {
            if (hurtSprite != null)
                sr.sprite = originalSprite;
            else
                sr.color = originalColor;
        }

        flashRoutine = null;
    }

}
