using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleFadeSequence : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup titleGroup; //this is where youll be dragging title PNG in inspector
    public CanvasGroup devGroup; //c'est la meme

    [Header("Timing (seconds)")]
    public float fadeInTime = 1.0f;
    public float holdTime = 1.0f;
    public float fadeOutTime = 1.0f;
    public float gapBetween = 0.3f;

    [Header("Scene Flow (optional)")]
    public bool autoLoadNextScene = false;
    public string nextSceneName = "02_Dialogue";

    void Start()
    {
        // start hidden
        titleGroup.alpha = 0f;
        devGroup.alpha = 0f;

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // title: fade in > paws > fade out
        yield return Fade(titleGroup, 0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(titleGroup, 1f, 0f, fadeOutTime);

        yield return new WaitForSeconds(gapBetween);

        // Dev: fade in > paws > fade out
        yield return Fade(devGroup, 0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(devGroup, 1f, 0f, fadeOutTime);

        if (autoLoadNextScene)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float t = 0f;
        group.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            group.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        group.alpha = to;
    }
}
