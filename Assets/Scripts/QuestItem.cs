using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public string itemId = "quest_item";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterItemCollected(itemId);
        }

        gameObject.SetActive(false);
    }
}