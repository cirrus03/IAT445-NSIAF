using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public enum QuestType
    {
        None,
        KillEnemies,
        CollectItem
    }

    [Header("Current Quest")]
    public QuestType currentQuest = QuestType.None;
    public bool questActive = false;
    public bool questComplete = false;

    [Header("Progress")]
    public int currentAmount = 0;
    public int requiredAmount = 0;
    public string currentQuestId = "";

    [Header("Quest Display")]
    public string currentQuestName = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartKillQuest(string questId, string questName, int killTarget)
    {
        currentQuest = QuestType.KillEnemies;
        currentQuestId = questId;
        currentQuestName = questName;
        requiredAmount = killTarget;
        currentAmount = 0;
        questActive = true;
        questComplete = false;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetObjective(currentQuestName, currentAmount + " / " + requiredAmount);
        }

        Debug.Log($"Started kill quest: {questName} ({killTarget})");
    }

    public void StartCollectQuest(string questId, string questName, int requiredCount = 1)
    {
        currentQuest = QuestType.CollectItem;
        currentQuestId = questId;
        currentQuestName = questName;
        requiredAmount = requiredCount;
        currentAmount = 0;
        questActive = true;
        questComplete = false;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetObjective(currentQuestName, currentAmount + " / " + requiredAmount);
        }

        Debug.Log($"Started collect quest: {questName} ({requiredCount})");
    }

    public void RegisterEnemyKilled()
    {
        if (!questActive || questComplete) return;
        if (currentQuest != QuestType.KillEnemies) return;

        currentAmount++;
        Debug.Log($"Enemy killed: {currentAmount}/{requiredAmount}");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetObjective(currentQuestName, currentAmount + " / " + requiredAmount);
        }

        CheckCompletion();
    }

    public void RegisterItemCollected(string itemId = "")
    {
        if (!questActive || questComplete) return;
        if (currentQuest != QuestType.CollectItem) return;

        currentAmount++;
        Debug.Log($"Item collected: {currentAmount}/{requiredAmount}");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetObjective(currentQuestName, currentAmount + " / " + requiredAmount);
        }

        CheckCompletion();
    }

    void CheckCompletion()
    {
        if (currentAmount >= requiredAmount)
        {
            questComplete = true;

            Debug.Log($"Quest complete: {currentQuestId}");

            if (QuestCompletePopup.Instance != null)
            {
                QuestCompletePopup.Instance.Show("Quest Completed");
            }
        }
    }

    public void ApplySaveData(SaveData data)
    {
        if (data == null) return;

        currentQuest = data.currentQuest;
        questActive = data.questActive;
        questComplete = data.questComplete;

        currentAmount = data.currentAmount;
        requiredAmount = data.requiredAmount;
        currentQuestId = data.currentQuestId;
        currentQuestName = data.currentQuestName;

        if (GameProgress.Instance != null)
        {
            if (!questActive)
            {
                GameProgress.Instance.ClearObjective();
            }
            else if (questComplete)
            {
                GameProgress.Instance.SetObjective("Talk to Crow");
            }
            else
            {
                GameProgress.Instance.SetObjective(currentQuestName, currentAmount + " / " + requiredAmount);
            }
        }
    }

    public void ClearQuest()
    {
        currentQuest = QuestType.None;
        currentQuestId = "";
        currentAmount = 0;
        requiredAmount = 0;
        questActive = false;
        questComplete = false;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ClearObjective();
        }
    }
}