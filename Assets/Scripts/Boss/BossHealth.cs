using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] public float maxHealth = 4;
    public float currentHealth { get; set; }
    public float MaxHealth => maxHealth;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthBarRoot;

    [Header("State")]
    public bool IsInvincible { get; private set; } = false;
    [SerializeField] private bool onlyTakeDamageWhenStunned = false;

    [Header("Damage Flash (optional)")]
    [SerializeField] private bool flashOnDamage = true;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private SpriteRenderer[] flashRenderers;
    [SerializeField] private Sprite hurtSprite;

    [Header("Death")]
    [SerializeField] private float deathDelay = 1f;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private GameObject levelExit;
    [SerializeField] private TextAsset inkJSON;

    private bool isDead = false;
    private bool hitInvincible = false;

    private BossStateMachine bossStateMachine;
    private Unity.Behavior.BehaviorGraphAgent agent;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D[] allColliders;
    private BossDamage[] damageHitboxes;

    private Color[] originalColors;
    private Sprite[] originalSprites;
    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;


        if (bossHealthBarRoot != null)
        {
            bossHealthBarRoot.SetActive(false);
        }
        bossStateMachine = GetComponent<BossStateMachine>();
        agent = GetComponent<Unity.Behavior.BehaviorGraphAgent>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        allColliders = GetComponentsInChildren<Collider2D>(true);
        damageHitboxes = GetComponentsInChildren<BossDamage>(true);

        CacheFlashDefaults();
    }

    private void CacheFlashDefaults()
    {
        if (flashRenderers == null || flashRenderers.Length == 0)
            return;

        originalColors = new Color[flashRenderers.Length];
        originalSprites = new Sprite[flashRenderers.Length];

        for (int i = 0; i < flashRenderers.Length; i++)
        {
            if (flashRenderers[i] == null) continue;

            originalColors[i] = flashRenderers[i].color;
            originalSprites[i] = flashRenderers[i].sprite;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        if (IsInvincible) return;
        if (hitInvincible) return;
        if (damageAmount <= 0f) return;

        // if the boss has a state machine, respect its special states
        if (bossStateMachine != null)
        {
            // signature phase = ignore damage
            if (bossStateMachine.IsSignatureActive())
                return;

            // only let the player damage the boss while stunned (can comment out if you want)
            // if (onlyTakeDamageWhenStunned && bossStateMachine.currentState != BossState.Stunned)
            //     return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Boss took {damageAmount} damage. HP: {currentHealth}");

        if (flashOnDamage)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamageRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "after_boss");
            levelExit.SetActive(true);
            return;
        }

        StartCoroutine(HitInvincibilityRoutine());
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
        Debug.Log("Boss invincible: " + value);
    }

    private IEnumerator HitInvincibilityRoutine()
    {
        hitInvincible = true;
        yield return new WaitForSeconds(0.1f);
        hitInvincible = false;
    }

    private IEnumerator FlashDamageRoutine()
    {
        if (flashRenderers == null || flashRenderers.Length == 0)
            yield break;

        for (int i = 0; i < flashRenderers.Length; i++)
        {
            if (flashRenderers[i] == null) continue;

            if (hurtSprite != null) flashRenderers[i].sprite = hurtSprite;
            else flashRenderers[i].color = Color.red;
        }

        yield return new WaitForSeconds(flashDuration);

        if (!isDead)
        {
            for (int i = 0; i < flashRenderers.Length; i++)
            {
                if (flashRenderers[i] == null) continue;

                if (hurtSprite != null) flashRenderers[i].sprite = originalSprites[i];
                else flashRenderers[i].color = originalColors[i];
            }
        }

        flashRoutine = null;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("BOSS DEAD");
        HideBossHealthBar();
        // disable behavior / movement
        if (bossStateMachine != null) bossStateMachine.enabled = false;
        if (agent != null) agent.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // disable damage hitboxes so boss cant hurt player while dead
        foreach (var dmg in damageHitboxes)
        {
            if (dmg != null) dmg.enabled = false;
        }

        // disable colliders
        foreach (var col in allColliders)
        {
            if (col != null) col.enabled = false;
        }

        BossEncounterController encounter = FindFirstObjectByType<BossEncounterController>();
        if (encounter != null)
        {
            encounter.MarkBossDefeated();
        }
        else if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level3BossDefeated = true;
        }
        // TODO:
        // - play animation
        // - trigger win condition
        if (anim != null)
            anim.SetTrigger("Die");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        if (destroyOnDeath)
            Destroy(gameObject);
    }

    public void ResetBoss()
    {
        StopAllCoroutines();

        isDead = false;
        IsInvincible = false;
        hitInvincible = false;
        currentHealth = maxHealth;

        HideBossHealthBar();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        foreach (var col in allColliders)
        {
            if (col != null) col.enabled = true;
        }

        foreach (var dmg in damageHitboxes)
        {
            if (dmg != null) dmg.enabled = true;
        }

        if (flashRenderers != null)
        {
            for (int i = 0; i < flashRenderers.Length; i++)
            {
                if (flashRenderers[i] == null) continue;

                flashRenderers[i].color = originalColors[i];
                flashRenderers[i].sprite = originalSprites[i];
            }
        }

        if (bossStateMachine != null)
        {
            bossStateMachine.enabled = true;
            bossStateMachine.SetState(BossState.Attack);
        }

        if (agent != null)
            agent.enabled = true;
    }
    public void ShowBossHealthBar()
    {
        if (bossHealthBarRoot != null)
            bossHealthBarRoot.SetActive(true);
    }

    public void HideBossHealthBar()
    {
        if (bossHealthBarRoot != null)
            bossHealthBarRoot.SetActive(false);
    }
}