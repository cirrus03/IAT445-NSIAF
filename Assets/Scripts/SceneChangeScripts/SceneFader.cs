using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image image;

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeInRoutine(duration));
    }

    IEnumerator FadeInRoutine(float duration)
    {
        float t = 0;
        Color c = image.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = 1f - (t / duration);
            image.color = c;
            yield return null;
        }

        c.a = 0;
        image.color = c;
    }

    public void FadeToBlack(float duration)
    {
        StartCoroutine(FadeToBlackRoutine(duration));
    }

    IEnumerator FadeToBlackRoutine(float duration)
    {
        float t = 0;
        Color c = image.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = t / duration;
            image.color = c;
            yield return null;
        }

        c.a = 1;
        image.color = c;
    }
}