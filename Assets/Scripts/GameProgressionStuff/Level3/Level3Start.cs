using UnityEngine;

public class Level3StartObjective : MonoBehaviour
{
    private void Start()
    {
        if (GameProgress.Instance == null)
            return;

        if (GameProgress.Instance.level3QuestStage <= 0)
        {
            GameProgress.Instance.level3QuestStage = 0;
            GameProgress.Instance.SetObjective("Explore");
        }
        else
        {
            RestoreObjective();
        }
    }

    private void RestoreObjective()
    {
        switch (GameProgress.Instance.level3QuestStage)
        {
            case 0:
                GameProgress.Instance.SetObjective("Explore");
                break;
            case 3:
                GameProgress.Instance.SetObjective("Find key");
                break;
            case 4:
                GameProgress.Instance.SetObjective("Return to door");
                break;
            case 5:
                GameProgress.Instance.SetObjective("...");
                break;
            default:
                GameProgress.Instance.SetObjective("Explore");
                break;
        }
    }
}