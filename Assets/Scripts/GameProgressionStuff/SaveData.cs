[System.Serializable]
public class SaveData
{
    public string sceneName;

    // GameProgress
    public int questStage;
    public bool dashUnlocked;
    public bool wallJumpUnlocked;
    public bool doubleJumpUnlocked;

    public int level2QuestStage;
    public bool level2LampAcquired;
    public bool level2PowerRestored;
    public bool level2BugQuestStarted;
    public bool level2BugQuestComplete;
    public int level2BugKillsCurrent;

    public GameProgress.MoodState playerMood;
    public bool scene3DoorKeyCollected;
    public int level3QuestStage;
    public bool level3HazardSpawnUnlocked;

    public bool level3BossRoomEntered;
    public bool level3BossFightStarted;
    public bool level3BossPlayerDiedHere;
    public bool level3BossDefeated;
    // QuestManager
    public QuestManager.QuestType currentQuest;
    public bool questActive;
    public bool questComplete;
    public int currentAmount;
    public int requiredAmount;
    public string currentQuestId;
    public string currentQuestName;

    // Checkpoint
    public string checkpointId;
}