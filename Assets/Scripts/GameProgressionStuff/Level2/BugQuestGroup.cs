using UnityEngine;

public class BugQuestGroup : MonoBehaviour
{
    [Header("Bug Quest")]
    [SerializeField] private int requiredKills = 3;
    [SerializeField] private int currentKilled = 0;
    [SerializeField] private bool questStarted = false;
    [SerializeField] private bool isComplete = false;

    public int RequiredKills => requiredKills;
    public int CurrentKilled => currentKilled;
    public bool IsComplete => isComplete;

    public void BeginQuest()
    {
        questStarted = true;
        currentKilled = 0;
        isComplete = false;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level2BugQuestStarted = true;
            GameProgress.Instance.level2BugQuestComplete = false;
            GameProgress.Instance.level2BugKillsCurrent = currentKilled;

            GameProgress.Instance.SetObjective("Clear the attic bugs", currentKilled + " / " + requiredKills);
        }

        Debug.Log("Bug quest started.");
    }
    public void RestoreQuestProgress(int savedKilled, bool savedComplete)
    {
        questStarted = true;
        currentKilled = Mathf.Clamp(savedKilled, 0, requiredKills);
        isComplete = savedComplete || currentKilled >= requiredKills;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level2BugQuestStarted = true;
            GameProgress.Instance.level2BugQuestComplete = isComplete;
            GameProgress.Instance.level2BugKillsCurrent = currentKilled;

            if (isComplete)
            {
                GameProgress.Instance.SetObjective("Talk to Fox");
            }
            else
            {
                GameProgress.Instance.SetObjective("Clear the attic bugs", currentKilled + " / " + requiredKills);
            }
        }

        Debug.Log($"Bug quest restored: {currentKilled}/{requiredKills}, complete = {isComplete}");
    }

    public void RegisterBugKilled()
    {
        if (!questStarted || isComplete)
            return;

        currentKilled++;

        Debug.Log($"Bug killed: {currentKilled}/{requiredKills}");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level2BugKillsCurrent = currentKilled;
            GameProgress.Instance.SetObjective("Clear the attic bugs", currentKilled + " / " + requiredKills);
        }

        if (currentKilled >= requiredKills)
        {
            isComplete = true;

            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.level2BugQuestComplete = true;
                GameProgress.Instance.level2BugKillsCurrent = currentKilled;
                GameProgress.Instance.SetObjective("Talk to Fox");
            }

            if (QuestCompletePopup.Instance != null)
            {
                QuestCompletePopup.Instance.Show("Bug Quest Complete");
            }
        }
    }
}