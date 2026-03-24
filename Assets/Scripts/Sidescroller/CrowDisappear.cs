using UnityEngine;

public class CrowDisappear : MonoBehaviour
{
    public Animator animator;

    public void DisableCrow()
    {
        animator.SetBool("isDisappearing", false); 
        gameObject.SetActive(false);
    }
}