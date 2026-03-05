using UnityEngine;

public class MobileJoystickUI : MonoBehaviour
{
    public RectTransform joystickRoot;
    public RectTransform joystickHandle;
    public float joystickRadius = 80f;

    void Start()
    {
        if (joystickRoot == null || joystickHandle == null)
        {
            Debug.LogError("Joystick references are missing!");

            Debug.Log("AWWWW");
            return;
        }

        // Only show joystick on mobile
        if (!Application.isMobilePlatform && !Application.isEditor)
        {
            joystickRoot.gameObject.SetActive(false);
            return;
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        Debug.Log(player);
        if (player != null)
        {
            player.SetupJoystick(joystickRoot, joystickHandle, joystickRadius);
        }
        else
        {
            Debug.LogError("PlayerController not found in scene!");
        }
    }
}