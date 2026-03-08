using UnityEngine;
using System.Collections.Generic;

public class MasterSparkBehaviour : MonoBehaviour
{
    private Transform player;
    private PlayerController playerController;

    private float duration;
    private float damage;
    private float hitCooldown;
    private float distanceFromPlayer;

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
    private bool sparkActive = false;

    [Header("Rotation")]
    public float rotationSpeed = 240f;

    private float currentAngle;
    private float targetAngle;

    [Header("Repel")]
    public LaserRepelCollider repelCollider;

    public void Initialize(
        Transform playerTransform,
        float dur,
        float dmg,
        float cooldown,
        float distance
    )
    {
        player = playerTransform;

        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        duration = dur;
        damage = dmg;
        hitCooldown = cooldown;
        distanceFromPlayer = distance;

        timer = duration;

        if (laserVisual == null)
            laserVisual = transform;

        baseScale = laserVisual.localScale;
        laserVisual.localScale = Vector3.zero;

        chargeTimer = chargeTime;

        transform.SetParent(player);

        if (playerController != null)
        {
            if (playerController.MoveDirection != Vector2.zero)
                lastMoveDirection = playerController.MoveDirection.normalized;
            else if (playerController.LastMoveDirection != Vector2.zero)
                lastMoveDirection = playerController.LastMoveDirection.normalized;
        }

        currentAngle =
            Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x)
            * Mathf.Rad2Deg + 90f;

        targetAngle = currentAngle;

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        Vector3 offset =
            new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0)
            * distanceFromPlayer;

        transform.localPosition = offset;

        if (repelCollider != null)
            repelCollider.player = player;
    }

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        if (playerController != null &&
            playerController.MoveDirection != Vector2.zero)
        {
            lastMoveDirection =
                playerController.MoveDirection.normalized;
        }

        targetAngle =
            Mathf.Atan2(
                lastMoveDirection.y,
                lastMoveDirection.x
            ) * Mathf.Rad2Deg + 90f;

        currentAngle =
            Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );

        transform.rotation =
            Quaternion.Euler(0, 0, currentAngle);

        float rad = (currentAngle - 90f) * Mathf.Deg2Rad;

        Vector3 offset =
            new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            ) * distanceFromPlayer;

        transform.localPosition = offset;

        if (!sparkActive)
        {
            chargeTimer -= Time.deltaTime;
            float progress = 1f - (chargeTimer / chargeTime);
            laserVisual.localScale =
                Vector3.Lerp(Vector3.zero, baseScale, progress);

            if (chargeTimer <= 0f)
                sparkActive = true;

            return;
        }

        if (timer > 0f)
        {
            float pulse =
                1f + Mathf.Sin(Time.time * pulseSpeed)
                * pulseAmount;

            laserVisual.localScale =
                new Vector3(
                    baseScale.x * pulse,
                    baseScale.y,
                    baseScale.z
                );

            timer -= Time.deltaTime;
        }
        else
        {
            chargeTimer += Time.deltaTime;
            float progress =
                Mathf.Clamp01(1f - (chargeTimer / chargeTime));

            laserVisual.localScale =
                Vector3.Lerp(Vector3.zero, baseScale, progress);

            if (progress <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!sparkActive)
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