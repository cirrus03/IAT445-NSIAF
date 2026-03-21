using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [SerializeField] private Transform respawnPoint;

    private bool isActivated = false;

    public void ActivateCheckpoint()
    {
        if (isActivated)
            return;

        isActivated = true;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(this);
        }

        Debug.Log("Activated checkpoint: " + name);
    }

    public Vector3 GetRespawnPosition()
    {
        if (respawnPoint != null)
            return respawnPoint.position;

        return transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        ActivateCheckpoint();
    }
}