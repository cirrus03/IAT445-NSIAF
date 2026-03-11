using UnityEngine;
using TMPro;

public class QuestTrackerUI : MonoBehaviour
{
    public TextMeshProUGUI questText;

    void Update()
    {
        if (QuestManager.Instance == null)
            return;

        var qm = QuestManager.Instance;

        if (!qm.questActive)
        {
            questText.text = "";
            return;
        }

        string progress = qm.currentAmount + " / " + qm.requiredAmount;

        questText.text =
            qm.currentQuestName + "\n" +
            progress;
    }
}