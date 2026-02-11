using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Quest Reference")]
    [SerializeField] private QuestControl questControl;

    private bool npcInRange;
    private void Update()
    {
   if (
            npcInRange &&
            !DialogueManager.GetInstance().dialogueIsPlaying &&
            (questControl == null || !questControl.QuestShown)
        )
        {
            if(visualCue !=null)// sorry i put these here because it was bugging out (apprently whatever its connecting to isnt connected)
            visualCue.SetActive(true);

            if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            if(visualCue !=null)//here too
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("NPC"))
        {
            npcInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("NPC"))
        {
            npcInRange = false;
            if(visualCue !=null)//here as well
            visualCue.SetActive(false);
        }
    }
}
