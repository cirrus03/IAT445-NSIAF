using UnityEngine;

public class SceneButtonTrigger : MonoBehaviour
{
    private void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
            return;

        if (DialogueManager.GetInstance().QuestIsActive &&
            InputManager.GetInstance().GetSubmitPressed())
        {
            SceneControl.instance.NextLevel();
        }

    }
}
