using UnityEngine;

public class IntroDialogueStarter : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;

    private void Start()
    {
        Debug.Log("Starting intro dialogue");
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
    }
}