using UnityEngine;

public class DoorKeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string pickupMessage = "You found a key.";
    [SerializeField] private bool destroyOnPickup = true;

    [Header("Optional")]
    [SerializeField] private GameObject pickupEffect;

    private bool collected = false;

    private void Start()
    {
        if (GameProgress.Instance != null && GameProgress.Instance.scene3DoorKeyCollected)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
            return;

        if (!collision.CompareTag("Player"))
            return;

        collected = true;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.scene3DoorKeyCollected = true;
            GameProgress.Instance.level3QuestStage = 4;
            GameProgress.Instance.SetObjective("Return to door");

            Debug.Log(pickupMessage);
        }
        else
        {
            Debug.LogWarning("GameProgress.Instance not found. Key state was not saved.");
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}