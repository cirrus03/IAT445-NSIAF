using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 3f;
    public float currentHealth { get; private set; }

    [Header("Drops")]
    [SerializeField] private GameObject healthDropPrefab;
    [SerializeField, Range(0f, 1f)] private float healthDropChance = 0.4f;

    [Header("Leetle guy spawn On Hit")]
    [SerializeField] private bool spawnFlyingEnemiesOnHit = false;
    [SerializeField] private GameObject flyingEnemySpawnedPrefab;
    [SerializeField] private int spawnCountOnHit = 2;
    [SerializeField] private float spawnCooldown = 0.5f;
    [SerializeField, Range(0f, 1f)] private float spawnChanceOnHit = 1f; // 1 = 100%
    [SerializeField] private float spawnMinRadius = 1.5f;
    [SerializeField] private float spawnMaxRadius = 3f;
    [SerializeField] private Transform[] spawnPoints;//optional — if empty, uses radius ring

    private float spawnCooldownTimer = 0f;

    [Header("Damage Flash (optional)")]
    [SerializeField] private bool flashOnDamage = true;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Quest Integration")]
    [SerializeField] private bool countsForBugQuest = false;

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

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
            originalSprite = sr.sprite;
        }
    }

    private void Update()
    {
        if (spawnCooldownTimer > 0f)
        {
            spawnCooldownTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damageAmount, Vector2 attackerPos)
    {
        if (isDying) return; //shouldnt be taking extra damage if youre dead (plan B)

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);

        if (flashOnDamage && sr != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashDamage());
        }

        // run away when hit, but only if still alive
        if (currentHealth > 0)
        {
            EnemyFlying flying = GetComponent<EnemyFlying>();
            if (flying != null)
            {
                flying.NotifyDamaged();
            }
            TrySpawnFlyingEnemiesOnHit();
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} died");

            if (countsForBugQuest)
            {
                BugQuestGroup bugQuest = FindFirstObjectByType<BugQuestGroup>();
                if (bugQuest != null)
                {
                    bugQuest.RegisterBugKilled();
                }
            }
            else
            {
                if (QuestManager.Instance != null)
                {
                    QuestManager.Instance.RegisterEnemyKilled();
                }
            }
            isDying = true;
            StartCoroutine(DieRoutine());
        }
    }
    public void TakeDamage(float damageAmount)
    {
        TakeDamage(damageAmount, transform.position); // no knockback direction
    }

    private void TrySpawnHealthDrop()
    {
        if (healthDropPrefab == null) return;

        if (Random.value <= healthDropChance)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }
    }

    private void TrySpawnFlyingEnemiesOnHit()
    {
        if (spawnCooldownTimer > 0f) return;
        if (flyingEnemySpawnedPrefab == null) return;
        if (!spawnFlyingEnemiesOnHit) return;

        if (Random.value > spawnChanceOnHit) return;

        spawnCooldownTimer = spawnCooldown;

        for (int i = 0; i < spawnCountOnHit; i++)
        {
            Vector3 spawnPos;

            if (spawnPoints != null && spawnPoints.Length > 0 && spawnPoints[i % spawnPoints.Length] != null)
            {
                spawnPos = spawnPoints[i % spawnPoints.Length].position;
            }
            else
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float distance = Random.Range(spawnMinRadius, spawnMaxRadius);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                spawnPos = (Vector2)transform.position + offset;
            }

            Instantiate(flyingEnemySpawnedPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void ResetEnemyState()
    {
        currentHealth = startingHealth;
        isDying = false;

        if (sr != null)
        {
            sr.color = originalColor;
            sr.sprite = originalSprite;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
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

        // disable all colliders so player can't hit or collide with corpse
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

        // show death flash
        if (sr != null)
        {
            if (hurtSprite != null) sr.sprite = hurtSprite;
            else sr.color = Color.red;
        }
        TrySpawnHealthDrop();
        
        yield return new WaitForSeconds(0.5f);

        if (GetComponent<EnemyResettable>() != null)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    IEnumerator FlashDamage()
    {
        if (sr == null) yield break;

        if (hurtSprite != null) sr.sprite = hurtSprite;
        else sr.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        // restores only when not dead
        if (!isDying)
        {
            if (hurtSprite != null) sr.sprite = originalSprite;
            else sr.color = originalColor;
        }

        flashRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnMinRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnMaxRadius);
    }
}
