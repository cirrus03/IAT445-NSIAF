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

    // If you assign this, it will temporarily swap to that sprite instead
    // If you leave it empty, it will just tint red instead.
    [SerializeField] private Sprite hurtSprite;

    private SpriteRenderer sr;
    private Color originalColor;
    private Sprite originalSprite;
    private Coroutine flashRoutine;

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
        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);

        if (flashOnDamage && sr != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} died");
            Destroy(gameObject);
        }
    }

    IEnumerator FlashDamage()
    {
        // if provided a red PNG sprite, swap to it.
        // otherwise just red
        if (hurtSprite != null)
        {
            sr.sprite = hurtSprite;
        }
        else
        {
            sr.color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        // Restore
        if (hurtSprite != null)
        {
            sr.sprite = originalSprite;
        }
        else
        {
            sr.color = originalColor;
        }

        flashRoutine = null;
    }
}
