using UnityEngine;

public class HotzoneCheck : MonoBehaviour
{
    private E_AttackerBehaviour enemyParent;
    private bool playerInRange;
    private Animator anim;

    private void Awake()
    {
        enemyParent = GetComponentInParent<E_AttackerBehaviour>();
        anim.GetComponentInParent<Animator>();

    }

    private void Update()
    {
        if(playerInRange && !anim.GetCurrentAnimatorStateInfo(1).IsName("atk1.1"))
        {
            // enemyParent.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerInRange= false;
            gameObject.SetActive(false);
            enemyParent.triggerArea.SetActive(true);
            enemyParent.inRange = false;
            // enemyParent.selectTarget();
        }
    }
}
