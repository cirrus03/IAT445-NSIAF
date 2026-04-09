using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 1f;

    [Header("Pogo")]
    public bool pogoOnHit = true;
    public float pogoBounceY = 14f;
    public bool pogoOnlyWhenFalling = false;
    public string hazardTag = "Hazard";

    [Header("Player Recoil (:joy_cat:)")]
    public bool recoilPlayerOnHit = false;
    public float recoilPlayerX = 5f;
    public float recoilPlayerY = 0f;
    public bool recoilUsesImpulse = false;
    public LayerMask recoilLayers; //set to whatever you want to recoil off of

    [Header("Hit VFX")]
    public GameObject hitVfxPrefab;
    public Vector3 hitVfxOffset = Vector3.zero;
    public bool spawnVfxOnEnemy = true;
    public bool spawnVfxOnWall = true;
    public bool spawnVfxOnGround = true;
    bool spawnedVfx = false;

    private SoundFXManager audioManager;

    Transform playerRoot;
    Collider2D hitboxCol;
    Rigidbody2D playerRb;
    bool smacked = false;

    void Awake()
    {
        playerRoot = transform.root;
        playerRb = playerRoot.GetComponent<Rigidbody2D>();
        hitboxCol = GetComponent<Collider2D>();

        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<SoundFXManager>();
        }
    }

    void OnEnable()
    {
        spawnedVfx = false;
        smacked = false;
        if (hitboxCol != null)
        {
            hitboxCol.enabled = true;
        }
    }

    private float GetFinalDamage()
    {
        float finalDamage = damage;

        if (GameProgress.Instance != null &&
            GameProgress.Instance.playerMood == GameProgress.MoodState.Happy)
        {
            finalDamage *= 2f;
        }

        return finalDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (smacked) return;

        // enemies only
        if (other.CompareTag("Player")) return;

        // damageable hit (enemy, breakable wall, etc.)
        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        // ENEMIES
        if (damageable != null && other.CompareTag("Enemy"))
        {
            if (spawnVfxOnEnemy && !spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }

            damageable.TakeDamage(GetFinalDamage());

            if (audioManager != null && audioManager.enemyHit != null)
            {
                audioManager.PlaySFX(audioManager.enemyHit, 0.2f);
            }

            ApplyPooogo();

            EnemyKnockback kb = other.GetComponentInParent<EnemyKnockback>();
            if (kb != null)
            {
                kb.Apply(playerRoot.position);
            }

            smacked = true;
            if (hitboxCol != null) hitboxCol.enabled = false;
            return;
        }

        // BREAKABLES
        if (damageable != null && other.CompareTag("Breakable"))
        {
            if (spawnVfxOnWall && !spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }

            damageable.TakeDamage(GetFinalDamage());

            if (audioManager != null && audioManager.hitWall != null)
            {
                audioManager.PlaySFX(audioManager.hitWall, 0.4f);
            }

            smacked = true;
            if (hitboxCol != null) hitboxCol.enabled = false;
            return;
        }

        // HAZARD POGO
        if (other.CompareTag(hazardTag))
        {
            if (!spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }
            if (audioManager != null && audioManager.hitWall != null)
            {
                audioManager.PlaySFX(audioManager.hitWall, 0.4f);
            }
            ApplyPooogo();
            return;
        }

        if (other.CompareTag("Ground"))
        {
            if (spawnVfxOnGround && !spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }

            if (audioManager != null && audioManager.hitWall != null)
            {
                audioManager.PlaySFX(audioManager.hitWall, 0.4f);
            }
            return;
        }

        // wall/enemy recoil (does NOT consume swing so enemy nearby can still be hit)
        if (recoilPlayerOnHit && playerRb != null)
        {
            bool inRecoilLayer = (recoilLayers.value & (1 << other.gameObject.layer)) != 0;

            if (inRecoilLayer)
            {
                if (spawnVfxOnWall && !spawnedVfx)
                {
                    SpawnHitVfx(other.ClosestPoint(transform.position));
                    spawnedVfx = true;
                }

                if (audioManager != null && audioManager.hitWall != null)
                {
                    audioManager.PlaySFX(audioManager.hitWall, 0.4f);
                }

                ApplyPlayerRecoil();
            }
        }
    }

    private void ApplyPooogo()
    {
        if (!pogoOnHit || playerRb == null) return;
        // if (pogoOnlyWhenFalling && playerRb.linearVelocity.y > 0f) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, pogoBounceY);

        PlayerMovement pm = playerRoot.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.RestoreOneJump();
        }
    }

    void SpawnHitVfx(Vector3 pos)
    {
        if (hitVfxPrefab == null)
        {
            return;
        }

        GameObject vfx = Instantiate(hitVfxPrefab, pos + hitVfxOffset, Quaternion.identity);

        float facing = Mathf.Sign(playerRoot.localScale.x);

        if (facing == 0)
        {
            facing = 1f;
        }

        Vector3 s = vfx.transform.localScale;
        s.x = Mathf.Abs(s.x) * (facing > 0 ? -1f : 1f);
        vfx.transform.localScale = s;
    }

    private void ApplyPlayerRecoil()
    {
        float facing = Mathf.Sign(playerRoot.localScale.x);
        if (facing == 0) facing = 1f;

        float recoilDir = -facing;
        Vector2 recoil = new Vector2(recoilDir * recoilPlayerX, recoilPlayerY);

        if (recoilUsesImpulse)
        {
            playerRb.AddForce(recoil, ForceMode2D.Impulse);
        }
        else
        {
            playerRb.linearVelocity = new Vector2(recoil.x, Mathf.Max(playerRb.linearVelocity.y, recoil.y));
        }

        PlayerMovement pm = playerRoot.GetComponent<PlayerMovement>();
        if (pm != null) pm.StartCoroutine(pm.RecoilLockRoutine(0.1f)); //recoil lock routine
    }
}