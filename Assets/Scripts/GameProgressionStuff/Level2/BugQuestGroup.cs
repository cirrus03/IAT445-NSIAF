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
            GameProgress.Instance.SetObjective("Clear the attic bugs", currentKilled + " / " + requiredKills);
        }
        Debug.Log("Bug quest started.");
    }

    public void RegisterBugKilled()
    {
        if (!questStarted || isComplete)
            return;

        currentKilled++;

        Debug.Log($"Bug killed: {currentKilled}/{requiredKills}");

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.SetObjective("Clear the attic bugs", currentKilled + " / " + requiredKills);
        }

        if (currentKilled >= requiredKills)
        {
            isComplete = true;

            if (GameProgress.Instance != null)
            {
                GameProgress.Instance.level2BugQuestComplete = true;
                GameProgress.Instance.SetObjective("Talk to Fox");
            }

            if (QuestCompletePopup.Instance != null)
            {
                QuestCompletePopup.Instance.Show("Bug Quest Complete");
            }
        }
    }
}