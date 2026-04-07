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

public class BossStateMachine : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 2f;
    private float dashTimer;

    [Header("Fly Attack")]
    public float flySpeed = 6f;
    public float flyDuration = 1.0f;
    public float flyCooldown = 3f;
    private float flyTimer;

    private bool isDashing = false;
    private bool isFlying = false;

    void Update()
    {
        //cooldowns tick down
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (flyTimer > 0) flyTimer -= Time.deltaTime;
    }


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