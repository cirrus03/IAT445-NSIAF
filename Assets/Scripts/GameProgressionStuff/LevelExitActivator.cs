using UnityEngine;

public class LevelExitActivator : MonoBehaviour
{
    public GameObject exitObject;

    public void ActivateExit()
    {
        if (exitObject != null)
        {
            exitObject.SetActive(true);
            Debug.Log("Time to go bye bye.");
        }
    }
}