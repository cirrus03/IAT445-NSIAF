using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame()
    {
        if (GameProgress.Instance == null)
        {
            Debug.LogWarning("No GameProgress instance found. Save failed.");
            return;
        }

        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.questStage = GameProgress.Instance.currentQuestStage;
        data.dashUnlocked = GameProgress.Instance.dashUnlocked;
        data.wallJumpUnlocked = GameProgress.Instance.wallJumpUnlocked;
        data.doubleJumpUnlocked = GameProgress.Instance.doubleJumpUnlocked;
        data.level2QuestStage = GameProgress.Instance.level2QuestStage;
        data.level2LampAcquired = GameProgress.Instance.level2LampAcquired;
        data.level2PowerRestored = GameProgress.Instance.level2PowerRestored;
        data.level2BugQuestStarted = GameProgress.Instance.level2BugQuestStarted;
        data.level2BugQuestComplete = GameProgress.Instance.level2BugQuestComplete;

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
    }
}