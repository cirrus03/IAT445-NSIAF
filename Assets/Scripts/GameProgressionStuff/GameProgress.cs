using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    [Header("Quest Tracker UI")]
    public string currentObjectiveText = "";
    public string currentObjectiveProgressText = "";

    [Header("Ability Unlocks")]
    public bool dashUnlocked = false;
    public bool wallJumpUnlocked = false;
    public bool doubleJumpUnlocked = false;

    [Header("Level 1 Quest Progression")]
    public int currentQuestStage = 0; // this is for level 1 only, fked pu the naming convention because i forgot wed have more levels and im too lazy to go and edit them all
    // 0 = not started
    // 1 = kill 1 enemy
    // 2 = collect first item / finished first quest
    // 3 = collect second item / finished second quest
    // 4 = all current quests done

    [Header("Level 2 Quest Progression")]
    public int level2QuestStage = 0;
    public bool level2LampAcquired = false;
    public bool level2PowerRestored = false;
    public bool level2BugQuestStarted = false;
    public bool level2BugQuestComplete = false;

    [Header("Flags")]
    public bool playerJustDied = false;

    [Header("Player MOOD")]
    public string playerMood = "neutral";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkPlayerDied()
    {
        playerJustDied = true;
    }

    public void ClearDeathFlag()
    {
        playerJustDied = false;
    }

    public void UnlockDash()
    {
        dashUnlocked = true;
    }

    public void UnlockWallJump()
    {
        wallJumpUnlocked = true;
    }

    public void UnlockDoubleJump()
    {
        doubleJumpUnlocked = true;
    }

    public void SetPlayerMood(string mood)
    {
        playerMood = mood;
    }

    public void SetObjective(string objective, string progress = "")
    {
        currentObjectiveText = objective;
        currentObjectiveProgressText = progress;
    }

    public void ClearObjective()
    {
        currentObjectiveText = "";
        currentObjectiveProgressText = "";
    }

    public void ResetAllProgress()
    {
        dashUnlocked = false;
        wallJumpUnlocked = false;
        doubleJumpUnlocked = false;

        currentQuestStage = 0;

        ResetLevel2Progress();

        playerJustDied = false;
        playerMood = "neutral";

        ClearObjective();
    }

    public void ResetLevel2Progress()
    {
        level2QuestStage = 0;
        level2LampAcquired = false;
        level2PowerRestored = false;
        level2BugQuestStarted = false;
        level2BugQuestComplete = false;
    }

    public void ApplySaveData(SaveData data)
    {
        if (data == null) return;

        currentQuestStage = data.questStage;
        dashUnlocked = data.dashUnlocked;
        wallJumpUnlocked = data.wallJumpUnlocked;
        doubleJumpUnlocked = data.doubleJumpUnlocked;

        level2QuestStage = data.level2QuestStage;
        level2LampAcquired = data.level2LampAcquired;
        level2PowerRestored = data.level2PowerRestored;
        level2BugQuestStarted = data.level2BugQuestStarted;
        level2BugQuestComplete = data.level2BugQuestComplete;
    }
}