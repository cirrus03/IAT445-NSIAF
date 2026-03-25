using UnityEngine;
using System.Collections;

public class IntroDialogueStarter : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;

    private void Start()
    {
        StartCoroutine(StartDialogueNextFrame());
    }

    private IEnumerator StartDialogueNextFrame()
    {
        yield return null; // wait one frame for everything to initialize

        if (DialogueManager.GetInstance() == null)
        {
            Debug.LogWarning("IntroDialogueStarter: DialogueManager not found.");
            yield break;
        }

        if (inkJSON == null)
        {
            Debug.LogWarning("IntroDialogueStarter: inkJSON is not assigned.");
            yield break;
        }

        Debug.Log("Starting intro dialogue");
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "intro");
    }
}