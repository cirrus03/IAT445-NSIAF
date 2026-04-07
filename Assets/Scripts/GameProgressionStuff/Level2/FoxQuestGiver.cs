using UnityEngine;

public class FoxQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;

    [Header("Dialogue")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Scene References")]
    [SerializeField] private Elevator mainToUpperElevator;
    [SerializeField] private BugQuestGroup bugQuestGroup;
    [SerializeField] private GameObject levelExitPortal;

    [Header("Downtime Dialogue")]
    [SerializeField]
    private string[] downtimeLines = {
    "The power is still out.",
    "Find the breaker.",
    "Darkness still reigns..."
};
    [SerializeField]
    private string[] downtimeLines2 =
    {
        "Goodness, the area is still swarming with bugs.",
        "They make my skin crawl... Please get rid of them.",
        "Are you sure they're all gone?"
    };
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
            bugQuestGroup.RestoreQuestProgress(
                GameProgress.Instance.level2BugKillsCurrent,
                GameProgress.Instance.level2BugQuestComplete
            );
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
                if (GameProgress.Instance.level2BugQuestComplete)
                {
                    GameProgress.Instance.SetObjective("Talk to Fox");
                }
                else if (bugQuestGroup != null)
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

    void PlayDowntimeDialogue(string[] lines)
    {
        DialogueManager dm = DialogueManager.GetInstance();
        if (dm == null || lines == null || lines.Length == 0) return;

        string line = lines[Random.Range(0, lines.Length)];
        dm.PlayDowntimeDialogue(line, Speaker.Fox);
    }

    private void Interact()
    {
        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress in scene.");
            return;
        }

        DialogueManager dm = DialogueManager.GetInstance();
        if (dm == null || dm.dialogueIsPlaying) return;

        int stage = GameProgress.Instance.level2QuestStage;

        // stage 0  intro
        if (stage == 0)
        {
            Debug.Log("Ink JSON is null? " + (inkJSON == null));
            dm.EnterDialogueMode(inkJSON, "platformer2");
            return;
        }

        // stage 1  breaker not done
        if (stage == 1)
        {
            PlayDowntimeDialogue(downtimeLines);
            return;
        }

        // stage 2  power restored 
        if (stage == 2)
        {
            dm.EnterDialogueMode(inkJSON, "firsttask_gotpower");
            return;
        }

        // stage 3 bug quest
        if (stage == 3)
        {
            if (bugQuestGroup != null && !bugQuestGroup.IsComplete)
            {
                PlayDowntimeDialogue(downtimeLines2);
            }
            else
            {
                dm.EnterDialogueMode(inkJSON, "secondtask_bugscleared");
            }
            return;
        }

        // stage 4 open portal
        if (stage == 4)
        {
            dm.EnterDialogueMode(inkJSON, "lasttask_enterportal");
            return;
        }

        // stage 5
        if (stage >= 5)
        {
            dm.EnterDialogueMode(inkJSON, "fox_done");
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