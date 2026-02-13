using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 1f;

    [Header("Enemy Knockback")]
    public float knockbackX = 13f;
    public float knockbackY = 3f;

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


    Transform playerRoot;
    Collider2D hitboxCol;
    Rigidbody2D playerRb;
    bool smacked = false;

    void Awake()
    {
        playerRoot = transform.root;
        playerRb = playerRoot.GetComponent<Rigidbody2D>();
        hitboxCol = GetComponent<Collider2D>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (smacked)
        {
            return;
        }
        // enemies only
        if (other.CompareTag("Player")) return;

        bool hitSomethingValid = false;

        //enemy hit
        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>(); 

        if (enemyHealth != null)
        {
            hitSomethingValid = true;

            if (spawnVfxOnEnemy && !spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }

            ApplyPooogo();

            enemyHealth.TakeDamage(damage);

            //enemy knockback
            Rigidbody2D enemyRb = other.attachedRigidbody;
            if (enemyRb != null)
            {

            float dir = Mathf.Sign(other.transform.position.x - playerRoot.position.x);
            if (dir == 0) dir = 1f;

            enemyRb.linearVelocity = Vector2.zero; // reset current motion
            enemyRb.AddForce(new Vector2(dir * knockbackX, knockbackY), ForceMode2D.Impulse);
            }

        }

        if (other.CompareTag(hazardTag)) //pogo off hazard
        {
            hitSomethingValid = true;
            if (!spawnedVfx)
            {
                SpawnHitVfx(other.ClosestPoint(transform.position));
                spawnedVfx = true;
            }
            ApplyPooogo();
        }

        // wall/neemy recoil
        if (recoilPlayerOnHit && playerRb != null)
        {
            bool inRecoilLayer = (recoilLayers.value & (1 << other.gameObject.layer)) != 0;

            if (inRecoilLayer)
            {
                hitSomethingValid = true;
                if (spawnVfxOnWall && !spawnedVfx)
                {
                    SpawnHitVfx(other.ClosestPoint(transform.position));
                    spawnedVfx = true;
                }
                ApplyPlayerRecoil();
            }
        }

        // consume swing if it its target
        if (hitSomethingValid)
        {
            smacked = true;
            if (hitboxCol != null) hitboxCol.enabled = false;
        }
    }
        private void ApplyPooogo()
    {
        if (!pogoOnHit || playerRb == null) return;
        if (pogoOnlyWhenFalling && playerRb.linearVelocity.y > 0f) return;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, pogoBounceY);
    }

    void SpawnHitVfx(Vector3 pos)
    {
        if (hitVfxPrefab == null)
        {
            return;
        }

        GameObject vfx = Instantiate(hitVfxPrefab, pos + hitVfxOffset, Quaternion.identity);
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
        if (pm != null) pm.StartCoroutine(pm.RecoilLockRoutine(0.1f));
    }

}
