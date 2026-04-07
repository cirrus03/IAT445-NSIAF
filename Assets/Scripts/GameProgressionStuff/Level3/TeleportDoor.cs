using System.Collections;
using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    [Header("Door Link")]
    [SerializeField] private TeleportDoor linkedDoor;
    [SerializeField] private Transform arrivalPoint;

    [Header("Lock Settings")]
    [SerializeField] private bool requiresKey = true;
    [SerializeField] private string lockedMessage = "It's locked. I need a key.";
    [SerializeField] private string interactMessage = "Press F to enter.";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Cooldown")]
    [SerializeField] private float teleportCooldown = 0.35f;

    private bool playerInRange = false;
    private bool isOnCooldown = false;
    private Transform playerTransform;

    private void Update()
    {
        if (!playerInRange || isOnCooldown)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        if (requiresKey && !HasRequiredKey())
        {
            Debug.Log(lockedMessage);

            if (GameProgress.Instance != null)
            {
                if (GameProgress.Instance.level3QuestStage < 3)
                {
                    GameProgress.Instance.level3QuestStage = 3;
                    GameProgress.Instance.SetObjective("Find key");
                }

                GameProgress.Instance.level3HazardSpawnUnlocked = true;
            }

            return;
        }

        if (linkedDoor == null)
        {
            Debug.LogWarning(gameObject.name + " has no linked door assigned.");
            return;
        }

        if (arrivalPoint == null)
        {
            Debug.LogWarning(gameObject.name + " has no arrival point assigned.");
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform not found.");
            return;
        }

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.level3QuestStage = 5;
            GameProgress.Instance.SetObjective("...");
        }

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        isOnCooldown = true;

        if (linkedDoor != null)
            linkedDoor.SetCooldown(teleportCooldown);

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        playerTransform.position = arrivalPoint.position;

        Debug.Log("Teleported to " + linkedDoor.gameObject.name);

        yield return new WaitForSeconds(teleportCooldown);
        isOnCooldown = false;
    }

    public void SetCooldown(float duration)
    {
        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(duration);
        isOnCooldown = false;
    }

    private bool HasRequiredKey()
    {
        if (GameProgress.Instance == null)
            return false;

        return GameProgress.Instance.scene3DoorKeyCollected;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = true;
        playerTransform = collision.transform;

        Debug.Log(interactMessage);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
    }
}