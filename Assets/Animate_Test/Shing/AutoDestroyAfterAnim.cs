using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    public float lifetime = 0.25f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
