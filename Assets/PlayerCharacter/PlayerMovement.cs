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
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;

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

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.5f;
    public bool dashGravityNono = true;
    private bool isDashing;
    private bool dashOnCD;
    private float dashGravitySaved;

    private bool okayButCanIDash = true;

    [Header("Ability Toggles (so we can use these as unlocks as abilities)")]
    public bool canDoubleJump = false;
    public bool canWallSlide = false;
    public bool canWallJump = false;
    public bool canDash = false;

    private bool controlsLocked = false; //setting so we can prevent movement (im assuming for pause menu or something)
    private Vector2 moveInput;

    private void OnEnable()
    {
        Health.PlayerDeath += DisablePlayerMovement;
    }
        private void OnDisable()
    {
        Health.PlayerDeath -= DisablePlayerMovement;
    }
    void Start()
    {
        ResetJumps();
        EnablePlayerMovement();
    }

    void Update()
    {
        if (controlsLocked) return;
        
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        // if (!isWallJumping)
        // {
        //     rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        //     Flip();
        // }
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        // animator.SetBool("isWallSliding", isWallSliding);
        if (!isWallJumping && !isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); //vert movement detection time baby
        horizontalMovement = moveInput.x;
    }

    public void Dash(InputAction.CallbackContext context)
    {

        if (!context.performed)
        {
            return;
        } 
        if (!canDash) 
        {
            return;
        }
        if (!okayButCanIDash)
        {
            return;
        }
        if (isDashing || dashOnCD) 
        {
            return;
        }

        okayButCanIDash = false;

        StartCoroutine(DashRoutine());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                // hold down jump for full height
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("Jump"); //mine is uppercase so it upper case :')
            }
            else if (context.canceled && rb.linearVelocity.y > 0)
            {
                // light tap of jump for short jump (half height)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }
        // wall jump
        if (context.performed && canWallJump && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);//jump away from wall
            wallJumpTimer = 0;
            animator.SetTrigger("Jump");

            ResetJumps();

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
    }

    public void Attack(InputAction.CallbackContext context)
    {
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
        if (moveInput.y > upDownThreshold)
        {
            currentAttackDirection = AttackDirection.Up;
            activeHitbox = attackHitboxUp;
        } 
        else if (moveInput.y < -upDownThreshold)
        {
            currentAttackDirection = AttackDirection.Down;
            activeHitbox = attackHitboxDown;
        }
        else
        {
            currentAttackDirection = AttackDirection.Side;
            activeHitbox = attackHitboxSide;
        }
        if (animator != null)//guys i swear switches are good 
        {
            switch (currentAttackDirection)
            {
                case AttackDirection.Up:
                animator.SetTrigger("AttackUp");
                break;

                case AttackDirection.Down:
                animator.SetTrigger("AttackDown");
                break;

                default:
                animator.SetTrigger("Attack");
                break;
            }
            
        }
        StartCoroutine(AttackRoutine(activeHitbox));
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            // Debug.Log("ground touched- reset jumps: " + Time.time);
            isGrounded = true;//reset upon landing
            ResetJumps();

            okayButCanIDash = true;//same
        }
        else
        {
            isGrounded = false;
        }
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
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
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
        jumpsRemaining = canDoubleJump ? maxJumps :1;
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

        yield return new WaitForSeconds(dashDuration);

        // end dash
        if (dashGravityNono)
            rb.gravityScale = dashGravitySaved;

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashOnCD = false;
    }

    private IEnumerator AttackRoutine(GameObject hitbox)
    {
        if (hitbox == null) yield break;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.1f); // active frames
        hitbox.SetActive(false);
    }


    //gizmos
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    //toggles for later

    public void SetDoubleJumpUnlocked(bool unlocked)
    {
        canDoubleJump = unlocked;
        ResetJumps();
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

    private void DisablePlayerMovement()
    {
        controlsLocked = true;
        rb.linearVelocity = Vector2.zero;//we stop movement with rb(rigidbody).linearVelocity to zero
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.enabled = false;
        // rb.bodyType =RigidbodyType2D.Static; //rigidbodies got error when you set them to static 
    }

        private void EnablePlayerMovement()
    {
        controlsLocked = false;
        animator.enabled = true;
        rb.bodyType =RigidbodyType2D.Dynamic;
    }

}
