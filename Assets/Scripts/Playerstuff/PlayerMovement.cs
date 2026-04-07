using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;

    [Header("MovementSpeed")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 15f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("Diegetic UI - Jump")]
    [SerializeField] private SpriteRenderer[] playerRenderers;
    [SerializeField] private Color fullJumpColor = Color.white;
    [SerializeField] private Color oneJumpLeftColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    [SerializeField] private Color noJumpsLeftColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;
    private bool wasGrounded;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Gravity")]
    public float baseGravity = 3f;
    public float maxFallSpeed = 25f;
    public float fallSpeedMultiplier = 1.5f;

    [Header("WallMovement")]
    public float wallSlideSpeed = 3;
    bool isWallSliding;

    [Header("Wall Jump")]
    bool isWallJumping;
    float wallJumpDirection;
    private float wallJumpTimer;
    public float wallJumpTime = 0.05f; // how long the wall jump window stays open
    public Vector2 wallJumpPower = new Vector2(5f, 10f); // x pushes away from wall

    [Header("Attack")]
    public float attackDamage = 1f;
    public float attackRange = 0.8f; // how far in front of player
    public Vector2 attackBoxSize = new Vector2(0.9f, 0.6f);
    public LayerMask enemyLayer;

    public float attackCooldown = 0.2f;
    private float lastAttackTime;

    //getters
    public bool DashOnCooldown => dashOnCD;
    public bool IsGrounded => isGrounded;
    public int JumpsRemaining => jumpsRemaining;
    public int MaxJumpsAvailable => canDoubleJump ? maxJumps : 1;
    public bool CanDashUnlocked => canDash;
    public bool CanDoubleJumpUnlocked => canDoubleJump;
    public bool CanWallJumpUnlocked => canWallJump;
    public bool IsTouchingWall => WallCheck();
    public float HorizontalInput => horizontalMovement;
    public int FacingDirection => isFacingRight ? 1 : -1;
    public bool CanDashNow => canDash && !isDashing && okayButHowManyDashes > 0 && !dashOnCD;
    //this is to grab dash cooldown if we have enough time
    private float dashCooldownTimer;
    public float DashCooldownTimer => dashCooldownTimer;
    public float DashCooldownDuration => dashCooldown;

    private enum AttackDirection //enums are like an array, super useful
    {
        Side,
        Up,
        Down
    }
    [SerializeField] private float upDownThreshold = 0.5f;
    private AttackDirection currentAttackDirection = AttackDirection.Side;
    private GameObject activeHitbox;

    [Header("Attack Hitboxes")]
    public GameObject attackHitboxSide;
    public GameObject attackHitboxUp;
    public GameObject attackHitboxDown;
    public GameObject attackHitboxDownAir;

    [Header("Hurtbox (Body Collider)")]
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private Vector2 normalHurtboxSize;
    [SerializeField] private Vector2 normalHurtboxOffset;
    [SerializeField] private Vector2 downAirHurtboxSize;
    [SerializeField] private Vector2 downAirHurtboxOffset;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.5f;
    public bool dashGravityNono = true;
    private bool isDashing;
    private bool dashOnCD;
    private float dashGravitySaved;

    private int okayButHowManyDashes = 1;

    [Header("Mood System")]
    [SerializeField] private float angryStillTimeRequired = 1.5f;
    [SerializeField] private GameObject protectionBubble;
    private float angryStillTimer = 0f;
    private bool angryInvincibilityActive = false;

    [Header("Ability Toggles (so we can use these as unlocks as abilities)")]
    public bool canDoubleJump = false;
    public bool canWallSlide = false;
    public bool canWallJump = false;
    public bool canDash = false;

    [Header("Recoil")]
    public bool recoilLock = false;


    private bool controlsLocked = false; //setting so we can prevent movement (im assuming for pause menu or something)
    private Vector2 moveInput;
    private Vector2 platformVelocity = Vector2.zero;

    private SoundFXManager audioManager; //audio player\
    [Header("Footsteps")]
    [SerializeField] private float footstepInterval = 0.35f;
    private float footstepTimer;

    private void Awake()
    {
        //assign the audio player
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<SoundFXManager>();
        }
    }


    private void OnEnable()
    {
        PlayerHealth.PlayerDeath += DisablePlayerMovement;
    }
    private void OnDisable()
    {
        PlayerHealth.PlayerDeath -= DisablePlayerMovement;
    }
    void Start()
    {
        ResetJumps();
        EnablePlayerMovement();
        if (GameProgress.Instance != null) // keeps unlocked information for future scenes
        {
            canDash = GameProgress.Instance.dashUnlocked;
            canWallJump = GameProgress.Instance.wallJumpUnlocked;
            canDoubleJump = GameProgress.Instance.doubleJumpUnlocked;
        }
        ResetDashCharges();

        if (bodyCollider != null)
        {
            normalHurtboxSize = bodyCollider.size;
            normalHurtboxOffset = bodyCollider.offset;
        }
        UpdateJumpTint();
    }

    void Update()
    {
        if (PauseMenu.isPaused || controlsLocked || IsGameplayFrozen())
        {
            moveInput = Vector2.zero;
            horizontalMovement = 0f;

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat("magnitude", 0f);
                animator.SetBool("isWallSliding", false);
            }

            GroundCheck();
            return;
        }

        GroundCheck();
        UpdateMoodEffects();
        HandleFootsteps();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        // if (!isWallJumping)
        // {
        //     rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        //     Flip();
        // }
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        float animMove = Mathf.Abs(horizontalMovement);

        if (WallCheck())
            animMove = 0f;

        animator.SetFloat("magnitude", animMove);

        animator.SetBool("isWallSliding", isWallSliding);
        if (!isWallJumping && !isDashing && !recoilLock)
        {
            float move = horizontalMovement;

            if (WallCheck() && Mathf.Sign(move) == Mathf.Sign(transform.localScale.x))
            {
                move = 0f;
            }
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || controlsLocked || IsGameplayFrozen())
        {
            moveInput = Vector2.zero;
            horizontalMovement = 0f;
            return;
        }
        moveInput = context.ReadValue<Vector2>(); //vert movement detection time baby
        horizontalMovement = moveInput.x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || IsGameplayFrozen() || controlsLocked)
        {
            return;
        }
        if (!context.performed)
        {
            return;
        }
        if (!canDash)
        {
            return;
        }
        if (okayButHowManyDashes <= 0)
        {
            return;
        }
        bool isSadMood = GetCurrentMood() == GameProgress.MoodState.Sad;

        if (isDashing)
        {
            return;
        }

        if (dashOnCD && !(isSadMood && okayButHowManyDashes > 1))
        {
            return;
        }

        okayButHowManyDashes--;

        StartCoroutine(DashRoutine());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || IsGameplayFrozen() || controlsLocked)
        {
            return;
        }
        if (isDashing)
        {
            return;//I'm sorry little one... you have been nerfed...(comment this line out if you want the weird dash jump thing back)
        }

        // wall jump
        if (context.performed && canWallJump && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);//jump away from wall
            wallJumpTimer = 0;
            animator.SetTrigger("Jump");

            jumpsRemaining = canDoubleJump ? 1 : 0;//oops accidentally gave three jumps

            // force flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            return;
        }

        if (context.performed && jumpsRemaining > 0)
        {
            // hold down jump for full height
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
            UpdateJumpTint();
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            // light tap of jump for short jump (half height)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.1f);
        }


    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused || IsGameplayFrozen() || controlsLocked)
        {
            return;
        }
        if (!context.performed)
        {
            return;
        }

        if (isWallSliding)
        {
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        // choose direction based on W/S
        bool useDownAirHurtbox = false;

        if (moveInput.y > upDownThreshold)
        {
            currentAttackDirection = AttackDirection.Up;
            activeHitbox = attackHitboxUp;
        }
        else if (moveInput.y < -upDownThreshold)
        {
            currentAttackDirection = AttackDirection.Down;

            if (isGrounded)
            {
                activeHitbox = attackHitboxDown;
            }
            else
            {
                activeHitbox = attackHitboxDownAir;
                useDownAirHurtbox = true;
            }
        }
        else
        {
            currentAttackDirection = AttackDirection.Side;
            activeHitbox = attackHitboxSide;
        }
        if (animator != null)
        {
            switch (currentAttackDirection)
            {
                case AttackDirection.Up:
                    animator.SetTrigger("AttackUp");
                    break;

                case AttackDirection.Down:
                    if (isGrounded)
                        animator.SetTrigger("AttackDown");
                    else
                        animator.SetTrigger("AttackDownAir");
                    break;

                default:
                    animator.SetTrigger("Attack");
                    break;
            }

            audioManager.PlaySFX(audioManager.playerAttack, 0.4f);

        }
        StartCoroutine(AttackRoutine(activeHitbox, useDownAirHurtbox));
    }

    private void GroundCheck()
    {
        bool groundedNow = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        // Debug.Log("ground touched- reset jumps: " + Time.time);
        isGrounded = groundedNow;

        if (groundedNow && !wasGrounded)
        {
            ResetDashCharges();//same
                               // audioManager.PlaySFX(audioManager.playerLand, 0.6f); //it's delayed rn 
            ResetJumps();
        }
        wasGrounded = groundedNow;//reset upon landing
    }

    private void HandleFootsteps()
    {
        if (isGrounded && Mathf.Abs(horizontalMovement) > 0.1f && !isDashing && !recoilLock)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (audioManager == null || audioManager.footstepSounds == null || audioManager.footstepSounds.Length == 0)
            return;

        int index = Random.Range(0, audioManager.footstepSounds.Length);
        audioManager.PlaySFX(audioManager.footstepSounds[index], 0.25f);
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void ProcessGravity()
    {
        if (isDashing) return;

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!canWallSlide)//gating it for later
        {
            isWallSliding = false;
            return;
        }

        // not grounded + on a wall + moving
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            if (!isWallSliding)
            {
                ResetDashCharges();
            }
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (!canWallJump)//gating for later (toggle)
        {
            wallJumpTimer = 0f;
            return;
        }

        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -Mathf.Sign(horizontalMovement); // push away from the wall
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void ResetJumps()
    {
        jumpsRemaining = canDoubleJump ? maxJumps : 1;
        UpdateJumpTint();
    }

    // taking mood from global
    private GameProgress.MoodState GetCurrentMood()
    {
        if (GameProgress.Instance == null)
            return GameProgress.MoodState.Neutral;

        return GameProgress.Instance.playerMood;
    }

    private void UpdateJumpTint()
    {
        if (playerRenderers == null || playerRenderers.Length == 0)
            return;

        Color targetColor = fullJumpColor;

        if (canDoubleJump)
        {
            if (jumpsRemaining <= 0)
            {
                targetColor = noJumpsLeftColor;
            }
            else if (jumpsRemaining == 1)
            {
                targetColor = oneJumpLeftColor;
            }
            else
            {
                targetColor = fullJumpColor;
            }
        }
        else
        {
            targetColor = fullJumpColor;
        }

        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
                playerRenderers[i].color = targetColor;
        }
    }

    // setting dashes to two on asd MOOD
    private int GetMaxDashCharges()
    {
        return GetCurrentMood() == GameProgress.MoodState.Sad ? 2 : 1;
    }

    // refresh dash count
    private void ResetDashCharges()
    {
        if (dashOnCD && GetCurrentMood() != GameProgress.MoodState.Sad)
            return;

        okayButHowManyDashes = GetMaxDashCharges();
    }

    // insta dash on sad
    private bool CanChainDashImmediately()
    {
        return GetMaxDashCharges() > 1;
    }

    // for the agnry MOOD
    // stand still for 2 seconds = invincible
    // instantly removed when upon movement
    private void UpdateMoodEffects()
    {
        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph == null) return;

        if (GetCurrentMood() != GameProgress.MoodState.Angry)
        {
            angryStillTimer = 0f;

            if (angryInvincibilityActive)
            {
                angryInvincibilityActive = false;
                ph.SetMoodInvincible(false);
                if (protectionBubble != null)
                    protectionBubble.SetActive(false);
            }

            return;
        }

        bool standingStill =
            Mathf.Abs(moveInput.x) < 0.01f &&
            moveInput.y <= 0f &&
            isGrounded &&
            !isDashing &&
            !isWallSliding &&
            !isWallJumping &&
            !recoilLock;

        if (standingStill)
        {
            angryStillTimer += Time.deltaTime;

            if (!angryInvincibilityActive && angryStillTimer >= angryStillTimeRequired)
            {
                angryInvincibilityActive = true;
                ph.SetMoodInvincible(true);
                if (protectionBubble != null)
                    protectionBubble.SetActive(true);
            }
        }
        else
        {
            angryStillTimer = 0f;

            if (angryInvincibilityActive)
            {
                angryInvincibilityActive = false;
                ph.SetMoodInvincible(false);
                if (protectionBubble != null)
                    protectionBubble.SetActive(false);
            }
        }
    }

    public void SetPlatformVelocity(Vector2 velocity)
    {
        platformVelocity = velocity;
    }

    public void ForceHazardInterrupt()
    {
        controlsLocked = true;
        recoilLock = true;

        // clear inputs 
        moveInput = Vector2.zero;
        horizontalMovement = 0f;

        // cancel active movement states
        isDashing = false;
        isWallSliding = false;
        isWallJumping = false;
        wallJumpTimer = 0f;

        CancelInvoke(nameof(CancelWallJump));

        rb.gravityScale = baseGravity;

        rb.linearVelocity = Vector2.zero;

        if (attackHitboxSide != null) attackHitboxSide.SetActive(false);
        if (attackHitboxUp != null) attackHitboxUp.SetActive(false);
        if (attackHitboxDown != null) attackHitboxDown.SetActive(false);
        if (attackHitboxDownAir != null) attackHitboxDownAir.SetActive(false);
        DisableDownAirHurtbox();
    }

    private bool IsGameplayFrozen()
    {
        // freeze player duruing dialogue
        if (DialogueManager.GetInstance() != null &&
            DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return true;
        }
        return false;
    }

    public void EnableAfterRespawn()
    {
        controlsLocked = false;
        recoilLock = false;
        isDashing = false;
        isWallSliding = false;
        isWallJumping = false;
        wallJumpTimer = 0f;

        moveInput = Vector2.zero;
        horizontalMovement = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
            animator.Rebind();
            animator.Update(0f);
        }

        angryStillTimer = 0f;
        angryInvincibilityActive = false;
        ResetDashCharges();

        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.SetMoodInvincible(false);
        }
        if (protectionBubble != null)
        {
            protectionBubble.SetActive(false);
        }

        ResetJumps();
        DisableDownAirHurtbox();
    }

    public IEnumerator RecoilLockRoutine(float t)
    {
        recoilLock = true;
        yield return new WaitForSeconds(t);
        recoilLock = false;
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashOnCD = true;

        // can disable gravity during dash
        if (dashGravityNono)
        {
            dashGravitySaved = rb.gravityScale;
            rb.gravityScale = 0f;
        }

        // dash direction = facing direciton
        float dir = isFacingRight ? 1f : -1f;

        // vert
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        // animation can add
        if (animator != null) animator.SetTrigger("Dash");
        audioManager.PlaySFX(audioManager.playerDash, 0.6f); //play dash sound

        yield return new WaitForSeconds(dashDuration);

        // end dash
        if (dashGravityNono)
            rb.gravityScale = dashGravitySaved;

        isDashing = false;

        // cooldown only applied after first dash is spent
        if (okayButHowManyDashes <= 0)
        {
            dashCooldownTimer = dashCooldown;
            while (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
                yield return null;
            }
            dashCooldownTimer = 0f;
        }

        dashOnCD = false;
        if (isGrounded || isWallSliding)
        {
            okayButHowManyDashes = GetMaxDashCharges();
        }
    }

    private IEnumerator AttackRoutine(GameObject hitbox, bool useDownAirHurtbox = false)
    {
        if (hitbox == null) yield break;

        if (useDownAirHurtbox)
            EnableDownAirHurtbox();

        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.1f); // active frames
        hitbox.SetActive(false);

        if (useDownAirHurtbox)
            DisableDownAirHurtbox();
    }

    private void EnableDownAirHurtbox()
    {
        if (bodyCollider == null) return;

        bodyCollider.size = downAirHurtboxSize;
        bodyCollider.offset = downAirHurtboxOffset;
    }

    private void DisableDownAirHurtbox()
    {
        if (bodyCollider == null) return;

        bodyCollider.size = normalHurtboxSize;
        bodyCollider.offset = normalHurtboxOffset;
    }

    public IEnumerator DamageStunRoutine(float duration)
    {
        recoilLock = true;

        yield return new WaitForSeconds(duration);

        recoilLock = false;
    }

    public IEnumerator HitStopRoutine(float duration)
    {
        // tiny freeze-frame (affects whole game). Keep it SHORT.
        float prev = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = prev;
    }

    public IEnumerator HazardRespawnRoutine(float lockDuration, float bounceY = 3f)
    {
        controlsLocked = true;
        recoilLock = true;

        if (animator != null)
            animator.enabled = false;

        rb.linearVelocity = Vector2.zero;

        yield return null; // lets teleport happen cleanly first

        if (bounceY > 0f)
            rb.linearVelocity = new Vector2(0f, bounceY);

        yield return new WaitForSeconds(lockDuration);

        if (animator != null)
            animator.enabled = true;

        recoilLock = false;
        controlsLocked = false;
    }

    //gizmos
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        if (bodyCollider == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position + (Vector3)bodyCollider.offset,
            bodyCollider.size
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position + (Vector3)downAirHurtboxOffset,
            downAirHurtboxSize
        );
    }

    //toggles for later

    public void SetDoubleJumpUnlocked(bool unlocked)
    {
        canDoubleJump = unlocked;
        ResetJumps();
        UpdateJumpTint();
    }

    public void SetWallSlideUnlocked(bool unlocked)
    {
        canWallSlide = unlocked;
        if (!unlocked) isWallSliding = false;
    }

    public void SetWallJumpUnlocked(bool unlocked)
    {
        canWallJump = unlocked;
        if (!unlocked) wallJumpTimer = 0f;
    }

    public void SetDashUnlocked(bool unlocked)
    {
        canDash = unlocked;
    }

    public void RestoreOneJump()
    {
        int maxAllowed = canDoubleJump ? maxJumps : 1;

        if (jumpsRemaining < maxAllowed)
        {
            jumpsRemaining++;
        }
        UpdateJumpTint();
    }

    private void DisablePlayerMovement()
    {
        controlsLocked = true;
        rb.linearVelocity = Vector2.zero;//we stop movement with rb(rigidbody).linearVelocity to zero
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.enabled = false;
        // rb.bodyType =RigidbodyType2D.Static; //rigidbodies got error when you set them to static 

        angryStillTimer = 0f;
        angryInvincibilityActive = false;

        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.SetMoodInvincible(false);
        }
        if (protectionBubble != null)
        {
            protectionBubble.SetActive(false);
        }
        DisableDownAirHurtbox();
    }

    private void EnablePlayerMovement()
    {
        controlsLocked = false;
        animator.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

}
