using UnityEngine;
using System.Collections.Generic;

public class LaserUltimateBehaviour : MonoBehaviour
{
    private Transform player;
    private PlayerController playerController;

    private float duration;
    private float damage;
    private float hitCooldown;

    private float timer;

    private Dictionary<Enemy, float> hitTimers =
        new Dictionary<Enemy, float>();

    private Vector2 lastMoveDirection = Vector2.right;

    [Header("Animation")]
    public Transform laserVisual;

    public float chargeTime = 0.4f;
    public float pulseSpeed = 8f;
    public float pulseAmount = 0.15f;

    private Vector3 baseScale;
    private float chargeTimer;
    private bool laserActive = false;

    // ================= INITIALIZE =================

    public void Initialize(
        Transform playerTransform,
        float dur,
        float dmg,
        float cooldown,
        float distanceFromPlayer
    )
    {
        player = playerTransform;

        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        duration = dur;
        damage = dmg;
        hitCooldown = cooldown;

        timer = duration;

        if (laserVisual == null)
            laserVisual = transform;

        baseScale = laserVisual.localScale;

        // Start as small circle
        laserVisual.localScale = Vector3.zero;

        chargeTimer = chargeTime;

        // Parent to player so it always follows player
        transform.SetParent(player);
        transform.localPosition = Vector3.zero;
    }

    // ================= UPDATE =================

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        // ================= GET PLAYER DIRECTION =================

        if (playerController != null &&
            playerController.MoveDirection != Vector2.zero)
            {
            lastMoveDirection = playerController.MoveDirection.normalized;
            }

        float angle =
        Mathf.Atan2(
            lastMoveDirection.y,
            lastMoveDirection.x
            ) * Mathf.Rad2Deg;

        // add 90 degree offset
        float correctedAngle = angle + 90f;

        Quaternion targetRotation =
            Quaternion.Euler(0, 0, correctedAngle);

        // Smooth rotation
        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                720f * Time.deltaTime
            );

        // ================= CHARGE ANIMATION =================

        if (!laserActive)
        {
            chargeTimer -= Time.deltaTime;

            float progress =
                1f - (chargeTimer / chargeTime);

            laserVisual.localScale =
                Vector3.Lerp(
                    Vector3.zero,
                    baseScale,
                    progress
                );

            if (chargeTimer <= 0f)
                laserActive = true;

            return;
        }

        // ================= PULSE ANIMATION =================

        float pulse =
            1f +
            Mathf.Sin(Time.time * pulseSpeed)
            * pulseAmount;

        laserVisual.localScale =
            new Vector3(
                baseScale.x,
                baseScale.y * pulse,
                baseScale.z
            );

        // ================= LIFETIME =================

        timer -= Time.deltaTime;

        if (timer <= 0f)
            Destroy(gameObject);
    }

    // ================= DAMAGE =================

    void OnTriggerStay2D(Collider2D other)
    {
        if (!laserActive)
            return;

        if (!other.CompareTag("Enemy"))
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy == null)
            return;

        float time = Time.time;

        if (!hitTimers.ContainsKey(enemy))
            hitTimers.Add(enemy, 0f);

        if (time - hitTimers[enemy] >= hitCooldown)
        {
            Vector2 dir =
                enemy.transform.position -
                transform.position;

            enemy.TakeDamage(
                (int)damage,
                dir,
                5f,
                0.2f
            );

            hitTimers[enemy] = time;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null &&
            hitTimers.ContainsKey(enemy))
        {
            hitTimers.Remove(enemy);
        }
    }
}