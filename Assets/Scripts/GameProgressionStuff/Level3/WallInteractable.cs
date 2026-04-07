using UnityEngine;

public class WallInteractable : MonoBehaviour
{
    [Header("Poster Message")]
    [TextArea]
    [SerializeField] private string message;

    [Header("Interaction")]
    [SerializeField] private GameObject interactPrompt;

    private bool playerInRange = false;

    private void Update()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(message);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}