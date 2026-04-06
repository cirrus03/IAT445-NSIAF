using UnityEngine;

public class AtticDarkZone : MonoBehaviour
{
    [SerializeField] private bool forceDarkInThisZone = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (DarknessController.Instance != null)
        {
            DarknessController.Instance.SetForceDark(forceDarkInThisZone);
        }
    }
}