using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarknessController : MonoBehaviour
{
    public static DarknessController Instance { get; private set; }

    [Header("Lights")]
    [SerializeField] private Light2D playerLight;
    [SerializeField] private Light2D globalLight;

    [Header("Player Light Settings")]
    [SerializeField] private float noLampRadius = 0f;
    [SerializeField] private float noLampIntensity = 0f;
    [SerializeField] private float lampRadius = 5f;
    [SerializeField] private float lampIntensity = 1.0f;

    [Header("Global Light Settings")]
    [SerializeField] private float darkGlobalIntensity = 0f;
    [SerializeField] private float powerOnGlobalIntensity = 1f;

    public bool HasLamp { get; private set; }
    public bool PowerRestored { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ApplySavedState();
    }

    public void GiveLamp()
    {
        HasLamp = true;

        if (GameProgress.Instance != null)
            GameProgress.Instance.level2LampAcquired = true;

        UpdateLight();
    }

    public void RestorePower()
    {
        PowerRestored = true;

        if (GameProgress.Instance != null)
            GameProgress.Instance.level2PowerRestored = true;

        UpdateLight();
    }

    public void RemoveLamp()
    {
        HasLamp = false;
        UpdateLight();
    }

    public void ApplySavedState()
    {
        if (GameProgress.Instance != null)
        {
            HasLamp = GameProgress.Instance.level2LampAcquired;
            PowerRestored = GameProgress.Instance.level2PowerRestored;
        }

        UpdateLight();
    }

    private void UpdateLight()
    {
        if (globalLight != null)
        {
            globalLight.intensity = PowerRestored ? powerOnGlobalIntensity : darkGlobalIntensity;
        }

        if (playerLight == null) return;

        if (PowerRestored)
        {
            playerLight.enabled = false;
            return;
        }

        playerLight.enabled = true;

        if (HasLamp)
        {
            playerLight.pointLightOuterRadius = lampRadius;
            playerLight.intensity = lampIntensity;
        }
        else
        {
            playerLight.pointLightOuterRadius = noLampRadius;
            playerLight.intensity = noLampIntensity;
        }
    }
}