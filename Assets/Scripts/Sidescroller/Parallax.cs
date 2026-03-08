using UnityEngine;

public class Parallax : MonoBehaviour
{
    Transform cam;
    Vector3 startPos;

    public float parallaxStrength = 0.3f;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        float distance = cam.position.x * parallaxStrength;
        transform.position = new Vector3(startPos.x + distance, startPos.y, startPos.z);
    }
}