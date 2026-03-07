using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Header("Movement")]
    public float maxSpeed = 5f;
    public float smoothness = 12f;

    [Header("Joystick (Mobile)")]
    RectTransform joystickRoot;
    RectTransform joystickHandle;
    float joystickRadius;

    private Rigidbody2D rb;

    // Touch variables
    private Vector2 touchInput;
    private int activeFingerId = -1;
    private bool isDragging;

    // Keyboard input
    private PlayerInputActions inputActions;
    private Vector2 keyboardInput;

    // ===== NEW =====
    private Vector2 lastMoveDirection = Vector2.right;
    public Vector2 LastMoveDirection => lastMoveDirection;

    public Vector2 MoveDirection => (touchInput + keyboardInput).normalized;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Setup Input System
        inputActions = new PlayerInputActions();

        if (joystickRoot != null)
            joystickRoot.gameObject.SetActive(false);

        // Apply Character Data (optional if using initializer)
        if (characterData != null)
        {
            maxSpeed = characterData.moveSpeed;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = characterData.characterSprite;
        }
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx =>
            keyboardInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Move.canceled += ctx =>
            keyboardInput = Vector2.zero;
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        HandleTouch();

        // ===== TRACK LAST MOVE DIRECTION =====
        Vector2 combined = touchInput + keyboardInput;
        if (combined != Vector2.zero)
        {
            lastMoveDirection = combined.normalized;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void HandleTouch()
    {
        if (Touchscreen.current == null)
            return;

        bool anyTouchPressed = false;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (!touch.press.isPressed)
                continue;

            anyTouchPressed = true;

            var phase = touch.phase.ReadValue();
            int fingerId = touch.touchId.ReadValue();
            Vector2 screenPos = touch.position.ReadValue();

            if (!isDragging && phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                isDragging = true;
                activeFingerId = fingerId;

                if (joystickRoot != null)
                {
                    joystickRoot.gameObject.SetActive(true);
                    joystickRoot.position = screenPos;
                }

                if (joystickHandle != null)
                    joystickHandle.localPosition = Vector2.zero;

                touchInput = Vector2.zero;
            }

            if (isDragging && fingerId == activeFingerId)
            {
                if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    Vector2 delta = screenPos - (Vector2)joystickRoot.position;
                    Vector2 clamped = Vector2.ClampMagnitude(delta, joystickRadius);

                    if (joystickHandle != null)
                        joystickHandle.localPosition = clamped;

                    touchInput = clamped / joystickRadius;
                }

                if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                    phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    StopJoystick();
                    return;
                }
            }
        }

        if (!anyTouchPressed)
            StopJoystick();
    }

    public void SetupJoystick(RectTransform root, RectTransform handle, float radius)
    {
        joystickRoot = root;
        joystickHandle = handle;
        joystickRadius = radius;

        if (joystickRoot != null)
            joystickRoot.gameObject.SetActive(false);
    }

    void StopJoystick()
    {
        isDragging = false;
        activeFingerId = -1;
        touchInput = Vector2.zero;

        if (joystickHandle != null)
            joystickHandle.localPosition = Vector2.zero;

        if (joystickRoot != null)
            joystickRoot.gameObject.SetActive(false);
    }

    void ApplyMovement()
    {
        // Combine keyboard + touch safely
        Vector2 finalInput = touchInput + keyboardInput;
        finalInput = Vector2.ClampMagnitude(finalInput, 1f);

        Vector2 targetVelocity = finalInput * maxSpeed;

        rb.linearVelocity = Vector2.Lerp(
            rb.linearVelocity,
            targetVelocity,
            smoothness * Time.fixedDeltaTime
        );
    }
}