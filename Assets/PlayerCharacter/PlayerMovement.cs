using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    [Header("Movement")]
    public float moveSpeed = 5f;
    float xMovement;
    public float drag = 0.9f;

    [Header("Jumping")]
    public float jumpPower = 10f;

    [Header("Ground checking")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.05f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed= 18f;
    public float fallSpeedMultiplier = 2f;

    InputAction moveAction;
    InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        // body.linearVelocity = moveAction.ReadValue<Vector2>();
        body.linearVelocity = new Vector2(xMovement*moveSpeed, body.linearVelocityY);
        HandleGravity();
    }

    void FixedUpdate()
    {
        if(isGrounded() && moveAction.ReadValue<Vector2>().x == 0) {
            body.linearVelocity *= drag;
            // Debug.Log(moveAction.ReadValue<Vector2>().x);
        }
        // Debug.Log(body.linearVelocityX);
    }

    public void Move(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded())
        {
            if (context.performed)
        {
            body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);

        } else if (context.canceled)
        {
            body.linearVelocity = new Vector2(body.linearVelocityX, body.linearVelocityY*0.5f);
            // Debug.Log(body.linearVelocity);
        }
        }

        
        

    }

    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {   
            // Debug.Log("grounded");
            return true;
        }
        // Debug.Log("not grounded");
        return false;
    }

    private void HandleGravity()
    {
        if(body.linearVelocityY < 0)
        {
            body.gravityScale = baseGravity*fallSpeedMultiplier;
            body.linearVelocity = new Vector2(body.linearVelocityX, Mathf.Max(body.linearVelocityY, -maxFallSpeed));
        } else
        {
            body.gravityScale = baseGravity;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
