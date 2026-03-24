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
                showComplete = true;
            else
                showInProgress = true;
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

    void Interact()
    {
        if (GameProgress.Instance == null || QuestManager.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress or QuestManager.");
            return;
        }

        int stage = GameProgress.Instance.currentQuestStage;
        QuestManager qm = QuestManager.Instance;

        // Stage 0 → intro + first quest
        if (stage == 0 && !qm.questActive)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "introplatformer");
            return;
        }

        // Stage 1 → waiting OR completed
        if (stage == 1)
        {
            if (qm.questActive && qm.questComplete)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "firsttaskdone");
            }
            return;
        }

        // Stage 2
        if (stage == 2)
        {
            if (qm.questActive && qm.questComplete)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "secondtaskdone");
            }
            return;
        }

        // Stage 3
        if (stage == 3)
        {
            if (qm.questActive && qm.questComplete)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "lasttaskdone");
            }
            return;
        }

        // Stage 4+
        if (stage >= 4)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "lasttaskdone");
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