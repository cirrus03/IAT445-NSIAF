using UnityEngine;

public class StaticHealthBob : MonoBehaviour
{
    public float bobHeight = 0.15f;
    public float bobSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = (Mathf.Sin(Time.time * bobSpeed) + 1f) * 0.5f * bobHeight;
        transform.position = startPos + new Vector3(0f, offset, 0f);
    }
}