using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    [Header("Ability Unlocks")]
    public bool dashUnlocked = false;
    public bool wallJumpUnlocked = false;
    public bool doubleJumpUnlocked = false;

    [Header("Quest Progression")]
    public int currentQuestStage = 0;
    // 0 = not started
    // 1 = kill 1 enemy
    // 2 = collect first item / finished first quest
    // 3 = collect second item / finished second quest
    // 4 = all current quests done

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

    public void ResetProgress()
    {
        dashUnlocked = false;
        wallJumpUnlocked = false;
        doubleJumpUnlocked = false;
        currentQuestStage = 0;
        playerJustDied = false;
        playerMood = "neutral";
    }

    public void ApplySaveData(SaveData data)
    {
        if (data == null) return;

        currentQuestStage = data.questStage;
        dashUnlocked = data.dashUnlocked;
        wallJumpUnlocked = data.wallJumpUnlocked;
        doubleJumpUnlocked = data.doubleJumpUnlocked;
    }
}