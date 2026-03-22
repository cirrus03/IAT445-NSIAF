using UnityEngine;

public class SetPosition : MonoBehaviour
{

    public Transform cameraPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = cameraPosition.position;
        Debug.Log("camera pos pos: " + cameraPosition.position);
        Debug.Log("current pos: " + this.transform.position);
    }

}
