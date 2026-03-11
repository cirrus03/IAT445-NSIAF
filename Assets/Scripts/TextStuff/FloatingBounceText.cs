using UnityEngine;

public class FloatingBounceText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatAmount = 1f;

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        rectTransform.anchoredPosition = startPos + new Vector2(0f, newY);
    }
}