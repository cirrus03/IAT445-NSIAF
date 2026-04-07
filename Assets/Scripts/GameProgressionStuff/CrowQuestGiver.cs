using UnityEngine;

public class CrowQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;
    [SerializeField] private TextAsset inkJSON;

    [Header("Crow Status Indicators")]
    public GameObject availableIndicator;   // ! : crow has something important
    public GameObject inProgressIndicator;  // ? : quest is active but not complete
    public GameObject completeIndicator;    // ! : quest ready to turn in

    [Header("Optional Ability Unlocker")]
    public AbilityUnlocker abilityUnlocker;
    public GameObject levelExitPortal;

    [Header("Downtime Dialogue")]
    [SerializeField]
    private string[] downtimeLines1 = {
    "Eliminating a single enemy is a rather easy task, is it not?",
    "There should be an enemy nearby, to the right.",
    "Return to us once you've eliminated an enemy."
};
    private string[] downtimeLines2 = {
    "Head upwards and retrieve your notes.",
    "There is a path upwards to the right of the lab.",
    "If needed, you can access the controls through the pause menu."
};

    private string[] downtimeLines3 = {
    "To the left, your final task awaits.",
    "Remember to make full use of your skillset.",
    "If you're truly ready for tomorrow's test, make haste."
};

    [SerializeField] private TutorialEnemyRespawner tutorialEnemyRespawner;

    private void Start()
    {
        RestoreLevel1ObjectiveFromProgress();
    }

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;

        UpdateIndicators();

        if (interactPrompt != null)
        {
            bool canShowPrompt = playerInRange &&
            DialogueManager.GetInstance() != null &&
            !DialogueManager.GetInstance().dialogueIsPlaying;

            interactPrompt.SetActive(canShowPrompt);
        }

        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    void UpdateIndicators()
    {
        if (GameProgress.Instance == null || QuestManager.Instance == null)
        {
            SetIndicatorState(false, false, false);
            return;
        }

        QuestManager qm = QuestManager.Instance;
        int stage = GameProgress.Instance.currentQuestStage;

        bool showAvailable = false;
        bool showInProgress = false;
        bool showComplete = false;

        if (GameProgress.Instance.playerJustDied)
        {
            showAvailable = true;
        }
        else if (stage == 0)
        {
            showAvailable = true;
        }
        else if (qm.questActive)
        {
            if (qm.questComplete)
            {
                showComplete = true;
                GameProgress.Instance.SetObjective("Talk to Crow");
            }
            else
            {
                showInProgress = true;
            }
        }
        else if (stage >= 4)
        {
            showAvailable = true;
        }

        SetIndicatorState(showAvailable, showInProgress, showComplete);
    }

    void SetIndicatorState(bool available, bool inProgress, bool complete)
    {
        if (availableIndicator != null && availableIndicator.activeSelf != available)
            availableIndicator.SetActive(available);

        if (inProgressIndicator != null && inProgressIndicator.activeSelf != inProgress)
            inProgressIndicator.SetActive(inProgress);

        if (completeIndicator != null && completeIndicator.activeSelf != complete)
            completeIndicator.SetActive(complete);
    }

    void PlayDowntimeDialogue(string[] lines)
    {
        DialogueManager dm = DialogueManager.GetInstance();
        if (dm == null || lines == null || lines.Length == 0) return;

        string line = lines[Random.Range(0, lines.Length)];
        dm.PlayDowntimeDialogue(line, Speaker.Crow);
    }

    void Interact()
    {
        if (GameProgress.Instance == null || QuestManager.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress or QuestManager.");
            return;
        }

        int stage = GameProgress.Instance.currentQuestStage;
        QuestManager qm = QuestManager.Instance;

        // stage 0
        if (stage == 0 && !qm.questActive)
        {
            if (tutorialEnemyRespawner != null)
            {
                tutorialEnemyRespawner.EnsureQuestEnemyExists();
            }
            else
            {
                Debug.LogWarning("tutorialEnemyRespawner is not assigned.");
            }
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "introplatformer");
            return;
        }

        // stage 1
        if (stage == 1)
        {
            if (qm.questActive && !qm.questComplete)
            {
                PlayDowntimeDialogue(downtimeLines1);
            }
            else
            {
                GameProgress.Instance.SetObjective("Talk to Crow");
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "firsttaskdone");
            }
            return;
        }

        // stage 2
        if (stage == 2)
        {
            if (qm.questActive && !qm.questComplete)
            {
                PlayDowntimeDialogue(downtimeLines2);
            }
            else
            {
                GameProgress.Instance.SetObjective("Talk to Crow");
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "secondtaskdone");
            }
            return;
        }

        // stage 3
        if (stage == 3)
        {
            if (qm.questActive && !qm.questComplete)
            {
                PlayDowntimeDialogue(downtimeLines3);
            }
            else
            {
                GameProgress.Instance.SetObjective("Talk to Crow");
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "lasttaskdone");
            }
            return;
        }

        // 4
        if (stage >= 4)
        {
            GameProgress.Instance.SetObjective("Enter the portal");
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "lasttaskdone");
        }
    }

    private void RestoreLevel1ObjectiveFromProgress()
    {
        if (GameProgress.Instance == null || QuestManager.Instance == null)
            return;

        int stage = GameProgress.Instance.currentQuestStage;
        QuestManager qm = QuestManager.Instance;

        if (qm.questActive && qm.questComplete)
        {
            GameProgress.Instance.SetObjective("Talk to Crow");
            return;
        }

        switch (stage)
        {
            case 0:
                GameProgress.Instance.ClearObjective();
                break;

            case 1:
            case 2:
            case 3:
                if (qm.questActive)
                    GameProgress.Instance.SetObjective(qm.currentQuestName, qm.currentAmount + " / " + qm.requiredAmount);
                else
                    GameProgress.Instance.SetObjective("Talk to Crow");
                break;

            case 4:
                GameProgress.Instance.SetObjective("Enter the portal");
                break;
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