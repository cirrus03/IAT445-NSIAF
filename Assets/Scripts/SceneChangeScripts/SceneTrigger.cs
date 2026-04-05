using UnityEngine;
using System.Collections;
using TMPro;

public class SceneTrigger : MonoBehaviour
{    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private string transitionText = "MINDSCAPE: The Crows' Laboratory";

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(Transition());
        }
    }

    private IEnumerator Transition()
    {
        // fade to blakc
        sceneFader.FadeToBlack(1.5f);

        yield return new WaitForSeconds(1.5f);

        // fade rexr
        yield return sceneFader.StartCoroutine(
            sceneFader.FadeTextRoutine(transitionText, 1f, 2f, 1f)
        );
        SceneControl.instance.NextLevel();
    }
}