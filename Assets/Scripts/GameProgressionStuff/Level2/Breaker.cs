using UnityEngine;

public class Breaker : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private bool playerInRange = false;

    [Header("State")]
    [SerializeField] private bool activated = false;

    [Header("Power Targets")]
    [SerializeField] private Elevator[] elevatorsToEnable;

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (activated)
        {
            Debug.Log("The power is already back on.");
            return;
        }

        activated = true;

        if (DarknessController.Instance != null)
            DarknessController.Instance.RestorePower();

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level2PowerRestored = true;
            GameProgress.Instance.level2QuestStage = 2;
        }

        if (elevatorsToEnable != null)
        {
            foreach (Elevator elevator in elevatorsToEnable)
            {
                if (elevator != null)
                    elevator.SetPoweredOn(true);
            }
        }

        Debug.Log("You switched the breaker back on.");
        Debug.Log("The lights ARE COMING BACK");
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