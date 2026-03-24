using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerMovement player;

    [Header("Dash UI")]
    [SerializeField] private GameObject dashContainer;
    [SerializeField] private Image dashIcon;
    [SerializeField] private Image dashCooldownFill;

    [Header("Double Jump UI")]
    [SerializeField] private GameObject doubleJumpContainer;
    [SerializeField] private Image doubleJumpIcon;

    [Header("Wall Jump UI")]
    [SerializeField] private GameObject wallJumpContainer;
    [SerializeField] private Image wallJumpIcon;

    [Header("Visuals")]
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color unavailableColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private void Update()
    {
        if (player == null) return;

        UpdateDashUI();
        UpdateDoubleJumpUI();
        UpdateWallJumpUI();
    }

    private void UpdateDashUI()
    {
        if (dashContainer != null)
            dashContainer.SetActive(player.CanDashUnlocked);

        if (!player.CanDashUnlocked) return;

        bool ready = !player.DashOnCooldown;

        if (dashIcon != null)
            dashIcon.color = ready ? readyColor : unavailableColor;

        if (dashCooldownFill != null)
        {
            if (ready)
            {
                dashCooldownFill.fillAmount = 0f;
            }
            else
            {
                float t = player.DashCooldownDuration > 0f
                    ? player.DashCooldownTimer / player.DashCooldownDuration
                    : 0f;

                dashCooldownFill.fillAmount = t;
            }
        }
    }

    private void UpdateDoubleJumpUI()
    {
        if (doubleJumpContainer != null)
            doubleJumpContainer.SetActive(player.CanDoubleJumpUnlocked);

        if (!player.CanDoubleJumpUnlocked) return;

        bool extraJumpAvailable = player.JumpsRemaining > 0;

        if (doubleJumpIcon != null)
            doubleJumpIcon.color = extraJumpAvailable ? readyColor : unavailableColor;
    }

    private void UpdateWallJumpUI()
    {
        if (wallJumpContainer != null)
            wallJumpContainer.SetActive(player.CanWallJumpUnlocked);

        if (!player.CanWallJumpUnlocked) return;

        bool touchingWall = player.IsTouchingWall && !player.IsGrounded;
        bool pressingTowardWall = false;

        if (touchingWall)
        {
            float input = player.HorizontalInput;
            int facing = player.FacingDirection;

            pressingTowardWall = Mathf.Abs(input) > 0.1f && Mathf.Sign(input) == facing;
        }

        if (wallJumpIcon != null)
            wallJumpIcon.color = (touchingWall && pressingTowardWall) ? readyColor : unavailableColor;
    }
}