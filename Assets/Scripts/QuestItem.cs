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

        if (QuestManager.Instance != null)
        {   
            audioManager.PlaySFX(audioManager.getQuestItem);
            QuestManager.Instance.RegisterItemCollected(itemId);
        }

        gameObject.SetActive(false);
    }
}