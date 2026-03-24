using UnityEngine;

public class MovingPlatformPassenger : MonoBehaviour
{
    private MovingPlatform movingPlatform;

    private void Awake()
    {
        movingPlatform = GetComponent<MovingPlatform>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                collision.transform.position += (Vector3)movingPlatform.DeltaMovement;
                return;
            }
        }
    }
}