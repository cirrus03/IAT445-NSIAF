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
    public GameObject attackHitbox;



    [Header("Ability Toggles (so we can use these as unlocks as abilities)")]
    public bool canDoubleJump = false;
    public bool canWallSlide = false;
    public bool canWallJump = false;

    void Start()
    {
        ResetJumps();
    }

    void Update()
    {
        
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        // animator.SetBool("isWallSliding", isWallSliding);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
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
                animator.SetTrigger("Jump");
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

        if (Time.time < lastAttackTime + attackCooldown) 
        {
            return;
        }

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
            

        // flip hitbox
        Vector3 scale = attackHitbox.transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        attackHitbox.transform.localScale = scale;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.1f); // active frames
        attackHitbox.SetActive(false);
    }



    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            isGrounded = true;
            ResetJumps();
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

}
