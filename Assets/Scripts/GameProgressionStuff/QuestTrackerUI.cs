using UnityEngine;
using TMPro;

public class QuestTrackerUI : MonoBehaviour
{
    public TextMeshProUGUI questText;

    void Update()
    {
        if (questText == null || GameProgress.Instance == null)
            return;

        string objective = GameProgress.Instance.currentObjectiveText;
        string progress = GameProgress.Instance.currentObjectiveProgressText;

        if (string.IsNullOrEmpty(objective))
        {
            questText.text = "";
            return;
        }

        if (string.IsNullOrEmpty(progress))
        {
            questText.text = objective;
        }
        else
        {
            questText.text = objective + "\n" + progress;
        }
    }
}