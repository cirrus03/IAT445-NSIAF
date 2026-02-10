using UnityEngine;
using UnityEngine.Video;

public class TriggerAreaCheck : MonoBehaviour
{

    private E_AttackerBehaviour enemyParent;
    

    private void Awake()
    {
        enemyParent = GetComponentInParent<E_AttackerBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if(collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyParent.target = collision.transform;
            enemyParent.inRange = true;
            enemyParent.hotZone.SetActive(true);
        }
    }
}