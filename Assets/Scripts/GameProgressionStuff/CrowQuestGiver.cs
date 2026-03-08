using UnityEngine;

public class CrowQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;

    [Header("Optional Ability Unlocker")]
    public AbilityUnlocker abilityUnlocker;
    public GameObject levelExitPortal;

    private void Update()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        if (!playerInRange) return;

        if (SimpleDialogueUI.Instance != null && SimpleDialogueUI.Instance.DialogueActive)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
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
                    "Crow: You made it.",
                    "Crow: I know encountering enemies can be scary.",
                    "Crow: But it's okay, dying here doesn't actually affect you... probably.",
                    "Crow: Anyways... I've got a quest for you.",
                    "Crow: Return to me once you finish.",
                    "Crow: I'll give you a little somethin' somethin'",
                    "Defeat 1 enemy.",
                    "You can LEFT CLICK to attack.",
                });

                qm.StartKillQuest("kill_first_enemy", 1);
                GameProgress.Instance.currentQuestStage = 1;
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
                    "Crow: Come back after defeating an enemy."
                });
                return;
            }

            if (qm.questActive && qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Well done.",
                    "Crow: Take this power.",
                    "You can dash now.",
                    "Press [RMB] or [SHIFT] to dash."
                });

                GameProgress.Instance.UnlockDash();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetDashUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 2;
                return;
            }
        }

        // Stage 2 -> second quest
        if (stage == 2)
        {
            if (!qm.questActive)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: I've got two more tasks for you before you can leave.",
                    "Crow: There is an item in the upper-right part of the map.",
                    "Crow: Bring it back to me."
                });

                qm.StartCollectQuest("collect_top_right_item", 1);
            }
            else if (!qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    $"Crow: You have found {qm.currentAmount} out of {qm.requiredAmount}.",
                    "Crow: The item is still out there."
                });
            }
            else
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Good.",
                    "Crow: I'll give you this ability next.",
                    "You've unlocked the wall jump ability."
                });

                GameProgress.Instance.UnlockWallJump();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetWallJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 3;
            }

            return;
        }

        // Stage 3 -> third quest
        if (stage == 3)
        {
            if (!qm.questActive)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Bring me one more item located in the top left quadrant of the area.",
                    "Crow: Then this final ability will be yours."
                });

                qm.StartCollectQuest("collect_final_item", 1);
            }
            else if (!qm.questComplete)
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    $"Crow: You have found {qm.currentAmount} out of {qm.requiredAmount}.",
                    "Crow: It doesn't look like you've found it yet."
                });
            }
            else
            {
                SimpleDialogueUI.Instance.StartDialogue(new string[]
                {
                    "Crow: Excellent.",
                    "Crow: The last ability is yours.",
                    "You can now double jump."
                });

                GameProgress.Instance.UnlockDoubleJump();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetDoubleJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 4;

                if (levelExitPortal != null)
                    levelExitPortal.SetActive(true);
            }

            return;
        }

        if (stage >= 4)
        {
            SimpleDialogueUI.Instance.StartDialogue(new string[]
            {
                "Crow: You are ready to continue.",
                "Crow: The portal back should open right about now."
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