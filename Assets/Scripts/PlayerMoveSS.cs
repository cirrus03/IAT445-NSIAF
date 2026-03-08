using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveSS : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    private Vector2 movement;
    void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        Vector2 dir = InputManager.GetInstance().GetMoveDirection();
        float input = dir.x;
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);

        if (input > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (input < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (input != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

    }

}
