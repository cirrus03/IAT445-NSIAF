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
    private void Start()
    {
        RestoreLevel2StateFromProgress();
    }
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

    private void RestoreLevel2StateFromProgress()
    {
        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress in scene.");
            return;
        }

        int stage = GameProgress.Instance.level2QuestStage;

        // lamp / darkness state
        if (GameProgress.Instance.level2LampAcquired)
        {
            if (DarknessController.Instance != null)
                DarknessController.Instance.GiveLamp();
        }

        // portal state
        if (levelExitPortal != null)
        {
            levelExitPortal.SetActive(stage >= 5 || GameProgress.Instance.level2BugQuestComplete);
        }

        // bug quest state
        if (bugQuestGroup != null && GameProgress.Instance.level2BugQuestStarted)
        {
            bugQuestGroup.BeginQuest();
        }

        // restoring objective text based on saved stage
        switch (stage)
        {
            case 0:
                GameProgress.Instance.ClearObjective();
                break;

            case 1:
                GameProgress.Instance.SetObjective("Find the breaker");
                break;

            case 2:
                GameProgress.Instance.SetObjective("Return to Fox");
                break;

            case 3:
                if (bugQuestGroup != null)
                {
                    GameProgress.Instance.SetObjective(
                        "Clear the attic bugs",
                        bugQuestGroup.CurrentKilled + " / " + bugQuestGroup.RequiredKills
                    );
                }
                else
                {
                    GameProgress.Instance.SetObjective("Clear the attic bugs");
                }
                break;

            case 4:
            case 5:
                GameProgress.Instance.SetObjective("Enter the portal");
                break;
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
            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.SetObjective("Find the breaker");
            }

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

            if (GameProgress.Instance != null && bugQuestGroup != null)
            {
                GameProgress.Instance.SetObjective("Clear the attic bugs", bugQuestGroup.CurrentKilled + " / " + bugQuestGroup.RequiredKills);
            }
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
            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.SetObjective("Enter the portal");
            }
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