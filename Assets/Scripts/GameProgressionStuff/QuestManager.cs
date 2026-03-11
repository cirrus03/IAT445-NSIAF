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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartKillQuest(string questId, int killTarget)
    {
        currentQuest = QuestType.KillEnemies;
        currentQuestId = questId;
        requiredAmount = killTarget;
        currentAmount = 0;
        questActive = true;
        questComplete = false;

        Debug.Log($"Started kill quest: {questId} ({killTarget})");
    }

    public void StartCollectQuest(string questId, int requiredCount = 1)
    {
        currentQuest = QuestType.CollectItem;
        currentQuestId = questId;
        requiredAmount = requiredCount;
        currentAmount = 0;
        questActive = true;
        questComplete = false;

        Debug.Log($"Started collect quest: {questId} ({requiredCount})");
    }

    public void RegisterEnemyKilled()
    {
        if (!questActive || questComplete) return;
        if (currentQuest != QuestType.KillEnemies) return;

        currentAmount++;
        Debug.Log($"Enemy killed: {currentAmount}/{requiredAmount}");

        CheckCompletion();
    }

    public void RegisterItemCollected(string itemId = "")
    {
        if (!questActive || questComplete) return;
        if (currentQuest != QuestType.CollectItem) return;

        currentAmount++;
        Debug.Log($"Item collected: {currentAmount}/{requiredAmount}");

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

    public void ClearQuest()
    {
        currentQuest = QuestType.None;
        currentQuestId = "";
        currentAmount = 0;
        requiredAmount = 0;
        questActive = false;
        questComplete = false;
    }
}