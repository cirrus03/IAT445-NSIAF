using UnityEngine;

public class FloatingBounceText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatAmount = 0.15f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.localPosition = startPos + new Vector3(0, newY, 0);
    }
}