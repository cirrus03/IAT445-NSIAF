// using UnityEngine;

// public class BossStateMachine : MonoBehaviour
// {
//     [Header("References")]
//     public Transform player;
//     private BossHealth health;
//     private Rigidbody2D rb;

//     [Header("Movement")]
//     public float moveSpeed = 3f;

//     [Header("Dash Attack")]
//     public float dashForce = 20f;
//     public float dashDuration = 0.2f;
//     public float dashCooldown = 2f;
//     private float dashTimer;

//     [Header("Fly Attack")]
//     public float flySpeed = 15f;
//     public float flyCooldown = 4f;
//     private float flyTimer;

//     [Header("Signature Move")]
//     public float summonCooldown = 10f;
//     private float summonTimer;

//     [Header("Phase")]
//     public bool isPhase2 = false;

//     private bool isBusy = false; // prevents overlapping actions

//     private void Awake()
//     {
//         health = GetComponent<BossHealth>();
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void Update()
//     {
//         // cooldown timers
//         if (dashTimer > 0f) dashTimer -= Time.deltaTime;
//         if (flyTimer > 0f) flyTimer -= Time.deltaTime;
//         if (summonTimer > 0f) summonTimer -= Time.deltaTime;

//         CheckPhaseTransition();
//     }


//     //player locations
//     // public float GetDistanceToPlayer()
//     // {
//     //     return Vector2.Distance(transform.position, player.position);
//     // }

//     public float GetDistanceToPlayer()
//     {
//         return Vector2.Distance(transform.position, player.position);
//     }

//     //graph conditions

//     public bool DashReady()
//     {
//         return dashTimer <= 0f && !isBusy;
//     }

//     public bool FlyReady()
//     {
//         return flyTimer <= 0f && !isBusy;
//     }

//     public bool SummonReady()
//     {
//         return summonTimer <= 0f && !isBusy;
//     }

//     //movement to player

//     public void MoveTowardsPlayer()
//     {
//         if (isBusy) return;

//         transform.position = Vector2.MoveTowards(
//             transform.position,
//             player.position,
//             moveSpeed * Time.deltaTime
//         );
//     }

//     //attacks

//     public void DashAttack()
//     {
//         if (!DashReady()) return;

//         Debug.Log("DASH ATTACK");

//         Vector2 dir = (player.position - transform.position).normalized;

//         rb.linearVelocity = Vector2.zero;
//         rb.AddForce(dir * dashForce, ForceMode2D.Impulse);

//         dashTimer = dashCooldown;
//         StartCoroutine(DashRoutine());
//     }

//     private System.Collections.IEnumerator DashRoutine()
//     {
//         isBusy = true;

//         yield return new WaitForSeconds(dashDuration);

//         rb.linearVelocity = Vector2.zero;
//         isBusy = false;
//     }

//     public void FlyAttack()
//     {
//         if (!FlyReady()) return;

//         Debug.Log("FLY ATTACK");

//         flyTimer = flyCooldown;

//         StartCoroutine(FlyRoutine());
//     }

//     private System.Collections.IEnumerator FlyRoutine()
//     {
//         isBusy = true;

//         int passes = isPhase2 ? 5 : 3;

//         for (int i = 0; i < passes; i++)
//         {
//             Vector2 dir = (player.position - transform.position).normalized;

//             rb.linearVelocity = dir * flySpeed;

//             yield return new WaitForSeconds(0.3f);
//         }

//         rb.linearVelocity = Vector2.zero;
//         isBusy = false;
//     }

//     public void StartSummonPhase()
//     {
//         if (!SummonReady()) return;

//         Debug.Log("SUMMON PHASE START");

//         summonTimer = summonCooldown;
//         StartCoroutine(SummonRoutine());
//     }

//     private System.Collections.IEnumerator SummonRoutine()
//     {
//         isBusy = true;

//         // Invincible during phase
//         if (health != null)
//             health.SetInvincible(true);

//         // TODO:
//         // spawn minions here
//         Debug.Log("Spawning minions...");

//         // Wait until minions are dead (placeholder)
//         yield return new WaitForSeconds(5f);

//         if (health != null)
//             health.SetInvincible(false);

//         Debug.Log("Boss stunned!");

//         // stun window
//         yield return new WaitForSeconds(3f);

//         isBusy = false;
//     }


//     //managing phases/transitions
//     private void CheckPhaseTransition()
//     {
//         if (health == null) return;

//         // if (!isPhase2 && health.CurrentHealth <= healthMaxHalf())
//         if (!isPhase2 && health.CurrentHealth <= health.MaxHealth * 0.5f)
//         {
//             EnterPhase2();
//         }
//     }

//     private float healthMaxHalf()
//     {
//         return health != null ? health.CurrentHealth + 0f : 0f; // placeholder fix below
//     }

//     private void EnterPhase2()
//     {
//         isPhase2 = true;

//         Debug.Log("PHASE 2 START");

//         StartCoroutine(Phase2Routine());
//     }

//     private System.Collections.IEnumerator Phase2Routine()
//     {
//         isBusy = true;

//         // TODO:
//         // - play charge animation
//         // - destroy tiles
//         Debug.Log("Charging... destroying arena");

//         yield return new WaitForSeconds(2f);

//         isBusy = false;
//     }
// }

using UnityEngine;

public enum BossState
{
    Idle,
    Attack,
    Signature,
    Stunned,
    PhaseTransition
}
public class BossStateMachine : MonoBehaviour
{



    public BossState currentState;
    public void SetState(BossState newState)
    {
        currentState = newState;
        Debug.Log("Switched to: " + newState);

        //will hoook later
    }

    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.7f;
    public float dashCooldown = 1f;
    private float dashTimer;

    [Header("Fly Attack")]
    public float flySpeed = 6f;
    public float flyDuration = 1.0f;
    public float flyCooldown = 3f;
    private float flyTimer;

    private bool isDashing = false;
    private bool isFlying = false;



    /// //////////////////////////////////////////////
    /// directions sprite is facing when attacking
    private bool isFacingRight = false; // matches EnemyFlying's default

    private void FaceDirection(Vector2 moveDirection)
    {
        if (moveDirection.x < -0.01f && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); // face right
            transform.localScale = scale;
        }
        else if (moveDirection.x > 0.01f && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // face left
            transform.localScale = scale;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////


    [Header("Signature Attack")]
    public GameObject minionPrefab;
    public Transform[] summonPoints;

    private int aliveMinions = 0;
    private bool isInvincible = false;
    private bool isPerformingSignature = false;




    void Start()
    {
        StartSignatureAttack(); //manually trigger for now , testign reasons
    }

    void Update()
    {
        //cooldowns tick down
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (flyTimer > 0) flyTimer -= Time.deltaTime;
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// signature attack minion spawn

    public void StartSignatureAttack()
    {
        if (isPerformingSignature) return;

        Debug.Log("SIGNATURE ATTACK START");

        isPerformingSignature = true;
        isInvincible = true;

        SpawnMinions();
    }

    public void EndSignature()
    {
        isInvincible = false;
        SetState(BossState.Stunned);
    }

    void SpawnMinions()
    {
        aliveMinions = 0;

        foreach (Transform point in summonPoints)
        {
            GameObject minion = Instantiate(minionPrefab, point.position, Quaternion.identity);

            EnemyHealth health = minion.GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.SetBossOwner(this);
            }

            aliveMinions++;
        }
    }

    public void OnMinionDied()
    {
        aliveMinions--;

        Debug.Log("Minion died. Remaining: " + aliveMinions);

        if (aliveMinions <= 0)
        {
            EndSignatureAttack();
        }
    }

    void EndSignatureAttack()
    {
        Debug.Log("SIGNATURE ATTACK END");

        isInvincible = false;
        isPerformingSignature = false;

        EnterStunnedState();
    }

    public void EnterStunnedState()
    {
        Debug.Log("BOSS STUNNED");

        // Later this will trigger FSM state switch
    }

    public bool IsSignatureActive()
    {
        return isPerformingSignature;
    }

    ///////////////////////////////////////////////////////////////////////////////////////\

    //conditions
    public float GetDistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    public bool DashReady()
    {
        return dashTimer <= 0f && !isDashing && !isFlying;
    }

    public bool FlyReady()
    {
        return flyTimer <= 0f && !isDashing && !isFlying;
    }


    //attacks
    public void DashAttack()
    {
        if (!DashReady()) return;

        Debug.Log("DASH ATTACK");

        dashTimer = dashCooldown;
        StartCoroutine(DashRoutine());
    }

    public void FlyAttack()
    {
        if (!FlyReady()) return;

        Debug.Log("FLY ATTACK");

        flyTimer = flyCooldown;
        StartCoroutine(FlyRoutine());
    }


    //COROUTINES
    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector2 direction = (player.position - transform.position).normalized;

        FaceDirection(direction); 

        float timer = 0f;

        while (timer < dashDuration)
        {
            transform.position += (Vector3)(direction * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    private System.Collections.IEnumerator FlyRoutine()
    {
        isFlying = true;

        float timer = 0f;

        while (timer < flyDuration)
        {
            // Move toward player but slower / floaty
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                flySpeed * Time.deltaTime
            );

            timer += Time.deltaTime;
            yield return null;
        }

        isFlying = false;
    }

    //move to player (default behaviour)

    public void MoveTowardsPlayer()
    {
        if (isDashing || isFlying) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }
}