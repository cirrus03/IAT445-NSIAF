using UnityEditor.AdaptivePerformance.Editor.Metadata;
using UnityEditor.SearchService;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class E_AttackerBehaviour : MonoBehaviour
{
   
   public float attackDistance; //minimum attack distance
   public float moveSpeed;
   public float timer; //cooldowsn between attacks
   public Transform leftLimit;
  public Transform rightLimit;

   [HideInInspector] public Transform target;
   [HideInInspector] public bool inRange;   //is player in range

   public GameObject hotZone;
   public GameObject triggerArea;


   private Animator animator;
   private float distance;  //distance b/w player and enemy
   private bool attackMode;
   private bool cooldown; //is enemy on cooldown after using attack
   private float intTimer;


    void Awake()
    {   
        SelectTarget();
        intTimer= timer;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(!attackMode)
        {
            EnemyMove();
        }

        if(inRange)
        {
            EnemyLogic();
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if(distance > attackDistance)
        {
            EnemyMove();
            EnemyStopAttack();
        }
        else if(attackDistance >= distance && !cooldown)
        {
            EnemyAttack();
        }

        if(cooldown)
        {   
            Cooldown();
            animator.SetBool("Attack", false);
        }
        
    
    }

    void EnemyMove()
    {   
        animator.SetBool("canWalk", true);
        if(animator.GetCurrentAnimatorStateInfo(1).IsName("atk1.1"))
        {
            Vector2 targetPostiion = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPostiion, moveSpeed*Time.deltaTime);
        }
    }

    void EnemyAttack()
    {
        timer = intTimer; //reset timer when players enter attack range
        attackMode = true;
        animator.SetBool("canWalk", false);
        animator.SetBool("Attack", true);
    }

    void EnemyStopAttack()
    {
        cooldown = false;
        attackMode = false;
        animator.SetBool("Attack", false);

    }

    public void triggerCooldown()
    {
        cooldown = true;
    }

    void Cooldown()
    {
        timer = -Time.deltaTime;
        if(timer <= 0 &&cooldown && attackMode)
        {
            cooldown = false;
            timer = intTimer; 
        }
    }

    private bool InsidePatrolLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x; 
    }

    private void Patrol()
    {
        if(!InsidePatrolLimits() && !inRange && !animator.GetCurrentAnimatorStateInfo(1).IsName("atk1.1"))
        {
            SelectTarget();
        }
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target= rightLimit;
        }

        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }

        transform.eulerAngles = rotation;
    }
}
