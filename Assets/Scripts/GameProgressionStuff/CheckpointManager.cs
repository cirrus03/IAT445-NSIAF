using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Checkpoint currentCheckpoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
        Debug.Log("Checkpoint set to: " + checkpoint.name);
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
            return currentCheckpoint.GetRespawnPosition();

        Debug.LogWarning("No checkpoint set. Using Vector3.zero.");
        return Vector3.zero;
    }

    public bool HasCheckpoint()
    {
        return currentCheckpoint != null;
    }

    public void RespawnPlayerAtCheckpoint(GameObject player)
    {
        if (player == null)
            return;

        if (currentCheckpoint == null)
        {
            Debug.LogWarning("No checkpoint set.");
            return;
        }

        Vector3 respawnPos = currentCheckpoint.GetRespawnPosition();
        player.transform.position = respawnPos;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.Revive();

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.EnableAfterRespawn();
    }
}