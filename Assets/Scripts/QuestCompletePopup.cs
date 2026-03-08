using System.Collections;
using UnityEngine;
using TMPro;

public class QuestCompletePopup : MonoBehaviour
{
    public static QuestCompletePopup Instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI popupText;

    public float fadeInTime = 0.2f;
    public float holdTime = 1f;
    public float fadeOutTime = 0.3f;

    void Awake()
    {
        Instance = this;
    }

    public void Show(string message)
    {
        popupText.text = message;
        StopAllCoroutines();
        StartCoroutine(PopupRoutine());
    }

    IEnumerator PopupRoutine()
    {
        // fade in
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        // fade out
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}