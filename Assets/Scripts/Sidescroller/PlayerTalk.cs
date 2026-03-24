using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private DialogueTrigger currentTrigger;

    private void Update()
    {
        if (currentTrigger != null)
        {
            // cue not alr triggered
            if (!currentTrigger.hasTriggered && currentTrigger.visualCue != null)
                currentTrigger.visualCue.SetActive(true);
            else if (currentTrigger.visualCue != null)
                currentTrigger.visualCue.SetActive(false);

            if (
                InputManager.GetInstance().GetInteractPressed() &&
                !DialogueManager.GetInstance().dialogueIsPlaying &&
                !currentTrigger.hasTriggered && 
                (currentTrigger.questControl == null || !currentTrigger.questControl.QuestShown)
            )
            {
                DialogueManager.GetInstance().EnterDialogueMode(
                    currentTrigger.inkJSON,
                    currentTrigger.knotName
                );

                currentTrigger.hasTriggered = true; 

                if (currentTrigger.visualCue != null)
                    currentTrigger.visualCue.SetActive(false);
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