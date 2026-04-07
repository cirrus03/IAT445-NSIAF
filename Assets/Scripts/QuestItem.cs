using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string itemId = "quest_item";
    private SoundFXManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundFXManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (QuestManager.Instance == null || GameProgress.Instance == null) return;

        if (itemId == "collect_top_right_item")
        {
            if (GameProgress.Instance.currentQuestStage <= 2)
            {
                if (GameProgress.Instance.currentQuestStage < 2)
                {
                    GameProgress.Instance.currentQuestStage = 2;
                }

                audioManager.PlaySFX(audioManager.getQuestItem);

                QuestManager.Instance.ForceCompleteQuest(
                    "collect_top_right_item",
                    "Collect the Upper-Right Item",
                    QuestManager.QuestType.CollectItem,
                    1
                );
            }

            gameObject.SetActive(false);
            return;
        }

        if (itemId == "collect_final_item")
        {
            if (GameProgress.Instance.currentQuestStage < 3)
            {
                GameProgress.Instance.currentQuestStage = 3;
            }

            audioManager.PlaySFX(audioManager.getQuestItem);

            QuestManager.Instance.ForceCompleteQuest(
                "collect_final_item",
                "Collect the Final Item",
                QuestManager.QuestType.CollectItem,
                1
            );

            gameObject.SetActive(false);
            return;
        }

        audioManager.PlaySFX(audioManager.getQuestItem);
        QuestManager.Instance.RegisterItemCollected(itemId);

        gameObject.SetActive(false);
    }
}