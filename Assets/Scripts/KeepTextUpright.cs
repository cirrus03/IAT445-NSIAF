using UnityEngine;

public class KeepTextUpright : MonoBehaviour
{
    void LateUpdate()
    {
        float parentX = transform.parent.lossyScale.x;

        Vector3 scale = transform.localScale;
        scale.x = parentX < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}