using UnityEngine;

public class BossFacePlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform visualToFlip;

    private void Update()
    {
        if (player == null || visualToFlip == null)
            return;

        FacePlayer();
    }

    private void FacePlayer()
    {
        float dx = player.position.x - visualToFlip.position.x;

        if (Mathf.Abs(dx) < 0.01f)
            return;

        Vector3 scale = visualToFlip.localScale;

        // because facing left sprite
        if (dx > 0f)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);

        visualToFlip.localScale = scale;
    }
}