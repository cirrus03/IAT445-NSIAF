using System;
using UnityEngine;

public class MeleeAtk : MonoBehaviour
{
    [SerializeField] private float atkCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float colliderDistance;
    [SerializeField] private LayerMask playerLayer;
    private float coolDownTimer = Mathf.Infinity;
    private PlayerHealth playerHealth;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {   

       
        coolDownTimer += Time.deltaTime;
        
        if(IsPlayerInSight()) {
            // Debug.Log("in sight");
            if(coolDownTimer >= atkCooldown)
            {
                coolDownTimer = 0;
                animator.SetTrigger("meleeAtk");
                // Debug.Log("attacked");
            }
        }
        
    }

    private bool IsPlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range *          transform.localScale.x * colliderDistance, 
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y *range, boxCollider.bounds.size.z), 
        0, Vector2.left, 0, playerLayer);  //origin, bounds, angle, direction, distance, layer

        if(hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<PlayerHealth>();
        }


        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right*range * transform.localScale.x * colliderDistance, 
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y *range, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if(IsPlayerInSight() )
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
