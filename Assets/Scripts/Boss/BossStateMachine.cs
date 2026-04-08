using UnityEngine;
using Unity.Behavior;
using System;


public enum BossState
{
    // Idle,
    Attack,
    Signature,
    Stunned,
    // PhaseTransition
}

public class BossStateMachine : MonoBehaviour
{

    public Animator BossAnimator;
    public BehaviorGraphAgent agent;

    [Header("Behavior Graphs")]
    // public BehaviorGraph idleGraph;
    public BehaviorGraph attackGraph;
    public BehaviorGraph signatureGraph;
    public BehaviorGraph stunnedGraph;
    // public BehaviorGraph phaseTransitionGraph;



    public BossState currentState;


    public void SetState(BossState newState)
    {
        currentState = newState;
        // Debug.Log("Switched to: " + newState);

        StopAllCoroutines(); // kill any in-flight dash/fly routines
    isDashing = false;   // reset flags since coroutines got killed
    isFlying = false;

        switch (newState)
        {
            case BossState.Attack:
                agent.Graph = attackGraph;
                // Debug.Log("Switched to: " + newState);
                break;

            case BossState.Signature:
                agent.Graph = signatureGraph;
                // Debug.Log("Switched to: " + newState);
                break;

            // case BossState.Stunned:
            //     agent.Graph = stunnedGraph;
            //     break;

            default:
                agent.Graph = attackGraph;
                break;

                // case BossState.PhaseTransition:
                //     agent.Graph = phaseTransitionGraph;
                //     break;
        }

        agent.Restart();
        InjectBlackboardVariables();
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

    BossHealth healthScript;
    private bool halfHealthTriggered = false;
    private bool lastHealthTriggered = false;



    [Header("Mmm Bubble")]
    public GameObject signatureShieldVisual;
    public Collider2D bossHurtbox;



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


    void Awake()
    {
        agent = GetComponent<BehaviorGraphAgent>();

        // agent.SetVariableValue("Boss", this);
        // agent.SetVariableValue("Player", player);
        InjectBlackboardVariables();

        //get references from enemy health
        healthScript = GetComponent<BossHealth>();
        if (healthScript == null)
        Debug.LogError("BossHealth component not found on " + gameObject.name);
    }

    void Start()
    {

        SetState(BossState.Attack);
    }

    void Update()
    {
        //cooldowns tick down
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (flyTimer > 0) flyTimer -= Time.deltaTime;

        //testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetState(BossState.Attack);

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetState(BossState.Signature);
            // StartSignatureAttack(); //turned this one here beacuse i didnt know where else to call it :')
        }

        //control listening for hp for zen mode change
        if (healthScript != null)
        {
            float maxHealth = healthScript.maxHealth;
            float currentHealth = healthScript.currentHealth;
            checkHealth(currentHealth, maxHealth);
        }

    }

    private void checkHealth(float currHealth, float maxHealth)
    {   
        // if(currentState == BossState.Signature)
        // {
        //      Debug.Log("already in sig");
        //     return;
        // }
           
        // if (currHealth == maxHealth / 2 || currHealth == 1)
        // {
        //     SetState(BossState.Signature);
        // }


    if (currentState == BossState.Signature) return;

    if (!halfHealthTriggered && currHealth == maxHealth / 2f)
    {
        halfHealthTriggered = true;
        Debug.Log("checking health has resulted switching to sig ver 1");
        SetState(BossState.Signature);
        return;
    }

    if (!lastHealthTriggered && currHealth <= 1f)
    {
        lastHealthTriggered = true;
        SetState(BossState.Signature);
    }
    }



    private void InjectBlackboardVariables()
    {
        agent.SetVariableValue("Boss", this);
        agent.SetVariableValue("Player", player);
        agent.SetVariableValue("BossAnimator", BossAnimator);
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// signature attack minion spawn

    public void StartSignatureAttack()
    {
        
        if (isPerformingSignature) return;

        Debug.Log("SIGNATURE ATTACK START");
        Debug.Log("Shield object assigned? " + (signatureShieldVisual != null));

        isPerformingSignature = true;
        isInvincible = true;

        if (signatureShieldVisual != null)
        {
            signatureShieldVisual.SetActive(true);
            Debug.Log("Shield turned on: " + signatureShieldVisual.activeSelf);
        }

        if (bossHurtbox != null)
            bossHurtbox.enabled = false;

        // SpawnMinions();
    }

    public void EndSignature()
    {
        isInvincible = false;
        SetState(BossState.Attack);
        Debug.Log("end zen mode switch to attack, notif 3/3");
    }

    public void SpawnMinions()
    {
        if(aliveMinions > 0) return;

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
            // SetState(BossState.Attack);
            EndSignatureAttack();
        }
    }

    public void EndSignatureAttack()
    {
        Debug.Log("SIGNATURE ATTACK END, switching to attack, end notif 1/3");
         EnterAttackState();

        isInvincible = false;
        isPerformingSignature = false;

        if (signatureShieldVisual != null)
            signatureShieldVisual.SetActive(false);

        if (bossHurtbox != null)
            bossHurtbox.enabled = true;

        // EnterAttackState();
    }

    public void EnterStunnedState()
    {
        Debug.Log("BOSS STUNNED");
        SetState(BossState.Stunned); //added for testing
        // Later this will trigger FSM state switch
    }

    public void EnterAttackState()
    {
        Debug.Log("ZEN MODE OVER, setting attack state, notif 2/3");
        SetState(BossState.Attack); //added for testing
      
    }

    public bool IsSignatureActive()
    {
        return isPerformingSignature;
    }


    /// //////////////////////////////////////
    /// 
    /// stun state
    public void startStun()
    {
        Debug.Log("Begin the stun state");
        Debug.Log("Play the falling down animation right here");
    }

    public void endStun()
    {
        Debug.Log("play the get up animation");
        Debug.Log("change state to attack");
        SetState(BossState.Attack); // adding for testing
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

            Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position);
            FaceDirection(toPlayer); //update flip every frame during fly

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

        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position);
        FaceDirection(toPlayer);

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }
}