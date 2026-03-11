using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    [Header("Trigger Type")]
    public bool triggerAutomatically = false;
    public bool requireInteract = true;
    public bool triggerOnce = true;

    [Header("Optional Prompt")]
    public GameObject interactPrompt;

    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string[] dialogueLines;

    private bool playerInRange = false;
    private bool hasTriggered = false;

    private void Update()
    {
        if (hasTriggered && triggerOnce) return;

        if (triggerAutomatically && playerInRange)
        {
            TriggerDialogue();
            return;
        }

        if (requireInteract && playerInRange)
        {
            if (interactPrompt != null && SimpleDialogueUI.Instance != null && !SimpleDialogueUI.Instance.DialogueActive)
                interactPrompt.SetActive(true);

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) &&
                SimpleDialogueUI.Instance != null &&
                !SimpleDialogueUI.Instance.DialogueActive)
            {
                TriggerDialogue();
            }
        }
        else
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void TriggerDialogue()
    {
        if (SimpleDialogueUI.Instance == null) return;
        if (SimpleDialogueUI.Instance.DialogueActive) return;

        hasTriggered = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        SimpleDialogueUI.Instance.StartDialogue(dialogueLines);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerInRange = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}