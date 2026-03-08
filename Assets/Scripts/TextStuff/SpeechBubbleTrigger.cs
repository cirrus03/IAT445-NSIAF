using UnityEngine;

public class SpeechBubbleTrigger : MonoBehaviour
{
    public GameObject bubble;

    private void Start()
    {
        bubble.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            bubble.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            bubble.SetActive(false);
        }
    }
}