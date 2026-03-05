using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UltimateController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;             
    public Button ultimateButton;         
    public Image cooldownImage;         

    private PlayerInputActions inputActions;
    private UltimateData equippedUltimate;

    private float cooldownTimer = 0f;
    private bool isReady = true;

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
        cooldownTimer = equippedUltimate.cooldown;

        if (ultimateButton != null)
            ultimateButton.interactable = false;
    }

    // ---------------- COOLDOWN ----------------

    void HandleCooldown()
    {
        if (isReady || equippedUltimate == null)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownImage != null)
            cooldownImage.fillAmount =
                cooldownTimer / equippedUltimate.cooldown;

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
}