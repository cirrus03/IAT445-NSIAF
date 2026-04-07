using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI fadeText;
    public static SceneFader Instance;
    public bool isFading { get; private set; }
    [SerializeField] private bool fadeOnStart = false;
    [SerializeField] private float startFadeDuration = 1.5f;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (fadeOnStart)
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;

            FadeIn(startFadeDuration);
        }
    }
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeInRoutine(duration));
    }

    public IEnumerator FadeTextRoutine(string message, float fadeInTime, float displayTime, float fadeOutTime)
    {
        fadeText.text = message;

        Color c = fadeText.color;
        c.a = 0;
        fadeText.color = c;

        // fade in
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            c.a = t / fadeInTime;
            fadeText.color = c;
            yield return null;
        }

        c.a = 1;
        fadeText.color = c;
        yield return new WaitForSeconds(displayTime);
        // fad eut
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            c.a = 1 - (t / fadeOutTime);
            fadeText.color = c;
            yield return null;
        }

        c.a = 0;
        fadeText.color = c;
    }

    IEnumerator FadeInRoutine(float duration)
    {
        isFading = true;
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
        isFading = false;
    }

    public void FadeToBlack(float duration)
    {
        StartCoroutine(FadeToBlackRoutine(duration));
    }

    IEnumerator FadeToBlackRoutine(float duration)
    {
        isFading = true;
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