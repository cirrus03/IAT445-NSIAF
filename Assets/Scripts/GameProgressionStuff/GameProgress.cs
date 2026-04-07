using UnityEngine;
//Because gameprogress is now enum based use these for setting MOOD!
// GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Neutral);
// GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Angry);
// GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Sad);
// GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Happy);
public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public enum MoodState
    {
        Neutral,
        Angry,
        Sad,
        Happy
    }


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

    [Header("Level 2 Bug Progress")]
    public int level2BugKillsCurrent = 0;

    [Header("Level 2 Quest Progression")]
    public int level2QuestStage = 0;
    public bool level2LampAcquired = false;
    public bool level2PowerRestored = false;
    public bool level2BugQuestStarted = false;
    public bool level2BugQuestComplete = false;

    [Header("Flags")]
    public bool playerJustDied = false;

    [Header("Player Mood")]
    public MoodState playerMood = MoodState.Neutral;

    [Header("Level 3 Quest Progression")]
    public int level3QuestStage = 0;
    public bool scene3DoorKeyCollected = false;

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

    public void SetPlayerMood(MoodState mood)
    {
        playerMood = mood;
        Debug.Log("Player mood set to: " + playerMood);
    }

    public bool IsMood(MoodState mood)
    {
        return playerMood == mood;
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
        playerMood = MoodState.Neutral;
        scene3DoorKeyCollected = false;
        level3QuestStage = 0;
        ClearObjective();
    }

    public void ResetLevel2Progress()
    {
        level2QuestStage = 0;
        level2LampAcquired = false;
        level2PowerRestored = false;
        level2BugQuestStarted = false;
        level2BugQuestComplete = false;
        level2BugKillsCurrent = 0;
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
        level2BugKillsCurrent = data.level2BugKillsCurrent;
        playerMood = data.playerMood;
        scene3DoorKeyCollected = data.scene3DoorKeyCollected;
        level3QuestStage = data.level3QuestStage;
    }

}