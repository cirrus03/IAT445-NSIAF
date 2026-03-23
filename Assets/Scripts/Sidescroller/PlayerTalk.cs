using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private DialogueTrigger currentTrigger;

    private void Update()
    {
        if (currentTrigger != null)
        {
            if (currentTrigger.visualCue != null)
                currentTrigger.visualCue.SetActive(true);

            if (
                InputManager.GetInstance().GetInteractPressed() &&
                !DialogueManager.GetInstance().dialogueIsPlaying &&
                (currentTrigger.questControl == null || !currentTrigger.questControl.QuestShown)
            )
            {
                DialogueManager.GetInstance().EnterDialogueMode(
                    currentTrigger.inkJSON,
                    currentTrigger.knotName
                );
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DialogueTrigger trigger = collision.GetComponent<DialogueTrigger>();

        if (trigger != null)
        {
            currentTrigger = trigger;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        DialogueTrigger trigger = collision.GetComponent<DialogueTrigger>();

        if (trigger != null && trigger == currentTrigger)
        {
            if (currentTrigger.visualCue != null)
                currentTrigger.visualCue.SetActive(false);

            currentTrigger = null;
        }
    }
}