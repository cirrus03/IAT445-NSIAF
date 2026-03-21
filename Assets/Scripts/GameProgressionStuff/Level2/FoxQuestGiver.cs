using UnityEngine;

public class FoxQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;

    [Header("Scene References")]
    [SerializeField] private Elevator mainToUpperElevator;
    [SerializeField] private BugQuestGroup bugQuestGroup;
    [SerializeField] private GameObject levelExitPortal;

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(playerInRange);
        }

        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress in scene.");
            return;
        }

        int stage = GameProgress.Instance.level2QuestStage;

        if (stage == 0)
        {
            Debug.Log("Fox: Gave lamp. Find the breaker.");

            if (DarknessController.Instance != null)
                DarknessController.Instance.GiveLamp();

            GameProgress.Instance.level2LampAcquired = true;
            GameProgress.Instance.level2QuestStage = 1;
            return;
        }

        if (stage == 1)
        {
            Debug.Log("Fox: Breaker is still off.");
            return;
        }

        if (stage == 2)
        {
            Debug.Log("Fox: Power restored. Start bug quest.");

            if (bugQuestGroup != null)
                bugQuestGroup.BeginQuest();

            GameProgress.Instance.level2BugQuestStarted = true;
            GameProgress.Instance.level2QuestStage = 3;
            return;
        }

        if (stage == 3)
        {
            if (bugQuestGroup != null && !bugQuestGroup.IsComplete)
            {
                Debug.Log($"Fox: Bugs killed {bugQuestGroup.CurrentKilled}/{bugQuestGroup.RequiredKills}");
            }
            else
            {
                Debug.Log("Fox: Bugs cleared.");

                GameProgress.Instance.level2BugQuestComplete = true;
                GameProgress.Instance.level2QuestStage = 4;
            }
            return;
        }

        if (stage == 4)
        {
            Debug.Log("Fox: Opening portal.");

            if (levelExitPortal != null)
                levelExitPortal.SetActive(true);

            GameProgress.Instance.level2QuestStage = 5;
            return;
        }

        if (stage >= 5)
        {
            Debug.Log("Fox: Done.");
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