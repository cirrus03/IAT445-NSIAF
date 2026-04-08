using UnityEngine;

public class BossAttackHitboxController : MonoBehaviour
{
    [SerializeField] private Collider2D swordHitbox;

    public void EnableSwordHitbox()
    {
        if (swordHitbox != null)
        {
            swordHitbox.enabled = true;
            Debug.Log("Sword hitbox ON");
        }
        else
        {
            Debug.LogWarning("Sword hitbox reference missing");
        }
    }

    public void DisableSwordHitbox()
    {
        if (swordHitbox != null)
        {
            swordHitbox.enabled = false;
            Debug.Log("Sword hitbox OFF");
        }
        else
        {
            Debug.LogWarning("Sword hitbox reference missing");
        }
    }
}