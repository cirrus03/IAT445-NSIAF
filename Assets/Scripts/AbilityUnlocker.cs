using UnityEngine;

public class AbilityUnlocker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerMovement player;

    public void UnlockWallJump()
    {
        player.SetWallJumpUnlocked(true);
    }

    public void UnlockWallSlide()
    {
        player.SetWallSlideUnlocked(true);
    }

    public void UnlockDoubleJump()
    {
        player.SetDoubleJumpUnlocked(true);
    }
}
