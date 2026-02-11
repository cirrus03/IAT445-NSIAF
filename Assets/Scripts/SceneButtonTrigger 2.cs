using UnityEngine;

public class SceneButtonTrigger : MonoBehaviour
{
    private void Update()
    {
        var dm = DialogueManager.GetInstance();
        var im = InputManager.GetInstance();

        if (dm == null || im == null)
            return;

        // Block scene change if dialogue is playing
        if (dm.dialogueIsPlaying)
            return;

        // Allow scene change if quest is active OR nothing is active
        if (im.GetSubmitPressed())
        {
            if (SceneControl.instance != null)
            {
                SceneControl.instance.NextLevel();
            }
            else
            {
                Debug.LogError("SceneControl.instance is null!");
            }
        }
    }
}
