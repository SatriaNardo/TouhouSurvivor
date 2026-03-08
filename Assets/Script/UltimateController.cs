using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class UltimateController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;             
    public Button ultimateButton;         
    public Image cooldownImage;

    private PlayerInputActions inputActions;
    private UltimateData equippedUltimate;

    [Header("DEBUG ULTIMATE STATS")]
    public UltimateData debugUltimate;
    public UltimateDebugStats debugStats;

    private float cooldownTimer = 0f;
    private bool isReady = true;

    // Runtime upgrade stats for each ultimate
    private Dictionary<UltimateData, UltimateRuntimeStats> runtimeStats
        = new Dictionary<UltimateData, UltimateRuntimeStats>();

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void Start()
    {
        SetupPlatformUI();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Ultimate.performed += OnUltimatePressed;
    }

    void OnDisable()
    {
        inputActions.Player.Ultimate.performed -= OnUltimatePressed;
        inputActions.Disable();
    }

    void Update()
    {
        HandleCooldown();
    }

    // ---------------- PLATFORM UI ----------------

    void SetupPlatformUI()
    {
        bool isMobile = Touchscreen.current != null;

        if (ultimateButton != null)
            ultimateButton.gameObject.SetActive(isMobile);
    }

    // ---------------- INPUT ----------------

    void OnUltimatePressed(InputAction.CallbackContext ctx)
    {
        UseUltimate();
    }

    // ---------------- EQUIP ----------------

    public void EquipUltimate(UltimateData ultimate)
    {
        equippedUltimate = ultimate;
        ResetCooldown();
    }

    // ---------------- USE ----------------

    public void UseUltimate()
    {
        if (!isReady || equippedUltimate == null || player == null)
            return;

        equippedUltimate.Activate(player);

        isReady = false;
        UltimateRuntimeStats stats = GetRuntimeStats(equippedUltimate);

        cooldownTimer = UltimateStatCalculator.GetStat(
                        equippedUltimate.cooldown,
                        UltimateUpgradeType.Cooldown,
                        stats
                        );

        if (ultimateButton != null)
            ultimateButton.interactable = false;
    }

    // ---------------- COOLDOWN ----------------

    void HandleCooldown()
    {
        if (isReady || equippedUltimate == null)
            return;

        UltimateRuntimeStats stats = GetRuntimeStats(equippedUltimate);

        cooldownTimer -= Time.deltaTime;

        if (cooldownImage != null)
            cooldownImage.fillAmount = cooldownTimer /
            UltimateStatCalculator.GetStat(
                        equippedUltimate.cooldown,
                        UltimateUpgradeType.Cooldown,
                        stats
                        );

        if (cooldownTimer <= 0f)
        {
            isReady = true;

            if (ultimateButton != null)
                ultimateButton.interactable = true;

            if (cooldownImage != null)
                cooldownImage.fillAmount = 0f;
        }
    }

    void ResetCooldown()
    {
        isReady = true;
        cooldownTimer = 0f;

        if (cooldownImage != null)
            cooldownImage.fillAmount = 0f;

        if (ultimateButton != null)
            ultimateButton.interactable = true;
    }
    public UltimateRuntimeStats GetRuntimeStats(UltimateData ultimate)
    {
        if (!runtimeStats.ContainsKey(ultimate))
        {
            UltimateRuntimeStats stats = new UltimateRuntimeStats();

            if (ultimate == debugUltimate)
            {
                stats.durationBonus = debugStats.durationBonus;
                stats.damageBonus = debugStats.damageBonus;
                stats.cooldownReduction = debugStats.cooldownReduction;

                stats.speedMultiplierBonus = debugStats.speedMultiplierBonus;
                stats.orbCountBonus = debugStats.orbCountBonus;
                stats.explosionRadiusBonus = debugStats.explosionRadiusBonus;
            }

            runtimeStats[ultimate] = stats;
        }

        return runtimeStats[ultimate];
    }
    public void UpgradeUltimate(UltimateUpgradeType type, float value)
    {
        if (equippedUltimate == null)
            return;

        UltimateRuntimeStats stats = GetRuntimeStats(equippedUltimate);
        stats.ApplyUpgrade(type, value);
    }
    public void ApplyDebugUpgrade()
    {
        if (debugUltimate == null) return;

        UltimateRuntimeStats stats = GetRuntimeStats(debugUltimate);

        stats.durationBonus += debugStats.durationBonus;
        stats.damageBonus += debugStats.damageBonus;
        stats.cooldownReduction += debugStats.cooldownReduction;

        stats.speedMultiplierBonus += debugStats.speedMultiplierBonus;
        stats.orbCountBonus += debugStats.orbCountBonus;
        stats.explosionRadiusBonus += debugStats.explosionRadiusBonus;

        Debug.Log("Debug upgrade applied.");
    }
}