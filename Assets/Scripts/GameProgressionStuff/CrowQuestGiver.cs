using UnityEngine;

public class CrowQuestGiver : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactPrompt;
    public bool playerInRange = false;

    [Header("Optional Ability Unlocker")]
    public AbilityUnlocker abilityUnlocker;

    private void Update()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (GameProgress.Instance == null || QuestManager.Instance == null)
        {
            Debug.LogWarning("Missing GameProgress or QuestManager in scene.");
            return;
        }

        // special death response first
        if (GameProgress.Instance.playerJustDied)
        {
            Debug.Log("Crow: Oh... so you died. It's okay, you can try again.");
            GameProgress.Instance.ClearDeathFlag();
            return;
        }

        int stage = GameProgress.Instance.currentQuestStage;
        QuestManager qm = QuestManager.Instance;

        // stage 0- give first quest
        if (stage == 0)
        {
            if (!qm.questActive)
            {
                Debug.Log("Crow: Defeat 1 enemy. Left click attacks — even in the air.");
                qm.StartKillQuest("kill_first_enemy", 1);
                GameProgress.Instance.currentQuestStage = 1;
            }
            return;
        }

        // First staage - first quest active / complete
        if (stage == 1)
        {
            if (qm.questActive && !qm.questComplete)
            {
                Debug.Log($"Crow: Come back after defeating 1 enemy. ({qm.currentAmount}/{qm.requiredAmount})");
                return;
            }

            if (qm.questActive && qm.questComplete)
            {
                Debug.Log("Crow: Well done. Take this — Dash is now yours.");
                GameProgress.Instance.UnlockDash();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetDashUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 2;
                return;
            }
        }

        // stage 2 - second quest
        if (stage == 2)
        {
            if (!qm.questActive)
            {
                Debug.Log("Crow: Find the item in the upper-right part of the map.");
                qm.StartCollectQuest("collect_top_right_item", 1);
            }
            else if (!qm.questComplete)
            {
                Debug.Log($"Crow: The item is still out there. ({qm.currentAmount}/{qm.requiredAmount})");
            }
            else
            {
                Debug.Log("Crow: Good. You earned Wall Jump.");
                GameProgress.Instance.UnlockWallJump();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetWallJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 3;
            }

            return;
        }

        // stage 3-  third quest
        if (stage == 3)
        {
            if (!qm.questActive)
            {
                Debug.Log("Crow: Bring me one more item.");
                qm.StartCollectQuest("collect_final_item", 1);
            }
            else if (!qm.questComplete)
            {
                Debug.Log($"Crow: Keep searching. ({qm.currentAmount}/{qm.requiredAmount})");
            }
            else
            {
                Debug.Log("Crow: Excellent. You now have Double Jump.");
                GameProgress.Instance.UnlockDoubleJump();

                if (abilityUnlocker != null)
                    abilityUnlocker.player.SetDoubleJumpUnlocked(true);

                qm.ClearQuest();
                GameProgress.Instance.currentQuestStage = 4;
            }

            return;
        }

        if (stage >= 4)
        {
            Debug.Log("Crow: You are ready to continue.");
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