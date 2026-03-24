using UnityEngine;

public class CrowQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;

    [Header("Crow Status Indicators")]
    public GameObject availableIndicator;   // ! : crow has something important
    public GameObject inProgressIndicator;  // ? : quest is active but not complete
    public GameObject completeIndicator;    // ! : quest ready to turn in

    [Header("Optional Ability Unlocker")]
    public AbilityUnlocker abilityUnlocker;
    public GameObject levelExitPortal;

    [SerializeField] private TutorialEnemyRespawner tutorialEnemyRespawner;

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;
        UpdateIndicators();

        if (interactPrompt != null)
        {
            bool canShowPrompt = playerInRange &&
                                 (SimpleDialogueUI.Instance == null || !SimpleDialogueUI.Instance.DialogueActive);
            interactPrompt.SetActive(canShowPrompt);
        }

        if (!playerInRange) return;

        if (SimpleDialogueUI.Instance != null && SimpleDialogueUI.Instance.DialogueActive)
            return;

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
        if (GameProgress.Instance == null || QuestManager.Instance == null || SimpleDialogueUI.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress, QuestManager, or SimpleDialogueUI in scene.");
            return;
        }

        int stage = GameProgress.Instance.currentQuestStage;
        QuestManager qm = QuestManager.Instance;

        // death response first
        if (GameProgress.Instance.playerJustDied)
        {
            SimpleDialogueUI.Instance.StartDialogue(new string[]
            {
                "Crow: Oh... so you died.",
                "Crow: It's okay.",
                "Crow: You can try again."
            });

            GameProgress.Instance.ClearDeathFlag();
            return;
        }

        // Stage 0 -> give first quest
        if (stage == 0)
        {
            if (!qm.questActive)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Dr. Duocrow White: Well, if it isn’t our prized pupil.",
                    "Dr. Duocrow Black: That’s right!",
                    "Crow: But it's okay, dying here doesn't actually affect you... probably.",
                    "Crow: Anyways... I've got a quest for you.",
                    "Crow: Return to me once you finish.",
                    "Crow: I'll give you a little somethin' somethin'.",
                    "Defeat 1 enemy.",
                    "You can [RMB] or [J] to attack."
                },
                () =>
                {

                    qm.StartKillQuest("kill_first_enemy", "Defeat 1 Enemy", 1);
                    GameProgress.Instance.currentQuestStage = 1;
                });
                if (tutorialEnemyRespawner != null)
                {
                    tutorialEnemyRespawner.EnsureQuestEnemyExists();
                }
            }
            return;
        }

        // Stage 1 -> first quest active / complete
        if (stage == 1)
        {
            if (qm.questActive && !qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    $"Crow: You have defeated {qm.currentAmount} out of {qm.requiredAmount}.",
                    "Crow: Come back after defeating an enemy, there should be one nearby."
                });
                return;
            }

            if (qm.questActive && qm.questComplete)
            {
                GameProgress.Instance.UnlockDash();

                if (abilityUnlocker != null && abilityUnlocker.player != null)
                    abilityUnlocker.player.SetDashUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 2;

                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Well done.",
                    "Crow: Take this power.",
                    "You can dash now.",
                    "Press [RMB] or [SHIFT] to dash.",
                    "Crow: Don't wander off just yet.",
                    "Crow: I've got another task for you.",
                    "Crow: There is an item in the upper-right part of the map.",
                    "Crow: Bring it back to me."
                },
                () =>
                {
                    qm.StartCollectQuest("collect_top_right_item", "Collect the Upper-Right Item", 1);
                });

                return;
            }
        }

        // Stage 2 -> second quest active / complete
        if (stage == 2)
        {
            if (qm.questActive && !qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    $"Crow: You have found {qm.currentAmount} out of {qm.requiredAmount}.",
                    "Crow: The item is still out there."
                });
                return;
            }

            if (qm.questActive && qm.questComplete)
            {
                GameProgress.Instance.UnlockWallJump();

                if (abilityUnlocker != null && abilityUnlocker.player != null)
                    abilityUnlocker.player.SetWallJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 3;

                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Good.",
                    "Crow: I'll give you this ability next.",
                    "You've unlocked the wall jump ability.",
                    "Crow: One last task remains.",
                    "Crow: Bring me the final item from the top-left part of the area."
                },
                () =>
                {
                    qm.StartCollectQuest("collect_final_item", "Collect the Final Item", 1);
                });

                return;
            }

            // safety fallback if somehow stage is 2 but no active quest
            if (!qm.questActive)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: There is an item in the upper-right part of the map.",
                    "Crow: Bring it back to me."
                },
                () =>
                {
                    qm.StartCollectQuest("collect_top_right_item", "Collect the Upper-Right Item", 1);
                });
                return;
            }
        }

        // Stage 3 -> third quest active / complete
        if (stage == 3)
        {
            if (qm.questActive && !qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    $"Crow: You have found {qm.currentAmount} out of {qm.requiredAmount}.",
                    "Crow: It doesn't look like you've found it yet."
                });
                return;
            }

            if (qm.questActive && qm.questComplete)
            {
                GameProgress.Instance.UnlockDoubleJump();

                if (abilityUnlocker != null && abilityUnlocker.player != null)
                    abilityUnlocker.player.SetDoubleJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 4;

                if (levelExitPortal != null)
                    levelExitPortal.SetActive(true);

                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Excellent.",
                    "Crow: The last ability is yours.",
                    "You can now double jump.",
                    "Crow: You are ready to continue.",
                    "Crow: The portal back should be open now."
                });

                return;
            }

            // safety fallback
            if (!qm.questActive)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Bring me one more item located in the top-left quadrant of the area."
                },
                () =>
                {
                    qm.StartCollectQuest("collect_final_item", "Collect the Final Item", 1);
                });
                return;
            }
        }

        if (stage >= 4)
        {
            SimpleDialogueUI.Instance.StartDialogue(new string[]
            {
                "Crow: You are ready to continue.",
                "Crow: The portal back should be open right about now."
            });
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