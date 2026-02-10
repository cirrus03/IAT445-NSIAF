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
            !questControl.QuestShown
        )
        {
            visualCue.SetActive(true);

            if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
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
            visualCue.SetActive(false);
        }
    }
}
