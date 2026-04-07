using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData PendingLoadData { get; private set; }

    public static void SaveGame()
    {
        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("No GameProgress instance found. Save failed.");
            return;
        }

        SaveData data = new SaveData();

        // scene
        data.sceneName = SceneManager.GetActiveScene().name;

        // GameProgress
        data.questStage = GameProgress.Instance.currentQuestStage;
        data.dashUnlocked = GameProgress.Instance.dashUnlocked;
        data.wallJumpUnlocked = GameProgress.Instance.wallJumpUnlocked;
        data.doubleJumpUnlocked = GameProgress.Instance.doubleJumpUnlocked;

        data.level2QuestStage = GameProgress.Instance.level2QuestStage;
        data.level2LampAcquired = GameProgress.Instance.level2LampAcquired;
        data.level2PowerRestored = GameProgress.Instance.level2PowerRestored;
        data.level2BugQuestStarted = GameProgress.Instance.level2BugQuestStarted;
        data.level2BugQuestComplete = GameProgress.Instance.level2BugQuestComplete;
        data.level2BugKillsCurrent = GameProgress.Instance.level2BugKillsCurrent;

        data.playerMood = GameProgress.Instance.playerMood;
        data.scene3DoorKeyCollected = GameProgress.Instance.scene3DoorKeyCollected;
        data.level3QuestStage = GameProgress.Instance.level3QuestStage;
        data.level3HazardSpawnUnlocked = GameProgress.Instance.level3HazardSpawnUnlocked;

        // QuestManager
        if (QuestManager.Instance != null)
        {
            data.currentQuest = QuestManager.Instance.currentQuest;
            data.questActive = QuestManager.Instance.questActive;
            data.questComplete = QuestManager.Instance.questComplete;
            data.currentAmount = QuestManager.Instance.currentAmount;
            data.requiredAmount = QuestManager.Instance.requiredAmount;
            data.currentQuestId = QuestManager.Instance.currentQuestId;
            data.currentQuestName = QuestManager.Instance.currentQuestName;
        }

        // checkpoint
        if (CheckpointManager.Instance != null && CheckpointManager.Instance.GetCurrentCheckpoint() != null)
        {
            data.checkpointId = CheckpointManager.Instance.GetCurrentCheckpoint().CheckpointID;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Game saved to: " + SavePath);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static void SetPendingLoadData(SaveData data)
    {
        PendingLoadData = data;
    }

    public static void ClearPendingLoadData()
    {
        PendingLoadData = null;
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save deleted.");
        }

        PendingLoadData = null;
    }
}