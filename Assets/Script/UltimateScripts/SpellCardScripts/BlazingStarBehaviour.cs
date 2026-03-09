using UnityEngine;
using System.Collections.Generic;

public class BlazingStarBehaviour : MonoBehaviour
{
    private Transform player;
    private PlayerController playerController;

    private float duration;
    private float damage;
    private float hitCooldown;
    private float speedMultiplier;

    private float timer;
    private float originalSpeed;

    public float rotationSpeed = 8f;

    private Dictionary<Enemy, float> hitTimers =
        new Dictionary<Enemy, float>();

    public void Initialize(
        Transform playerTransform,
        float dur,
        float dmg,
        float cooldown,
        float speedMulti
    )
    {
        player = playerTransform;

        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        duration = dur;
        damage = dmg;
        hitCooldown = cooldown;
        speedMultiplier = speedMulti;

        timer = duration;

        transform.SetParent(player);
        transform.localPosition = Vector3.zero;

        if (playerController != null)
        {
            originalSpeed = playerController.maxSpeed;

            playerController.StartForcedSpeed(originalSpeed * speedMultiplier);

            Vector2 dir = playerController.LastMoveDirection;

            float angle =
                Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 225f;

            transform.localRotation =
                Quaternion.Euler(0, 0, angle);
        }
    }

    void Update()
    {
        if (player == null)
        {
            Destroy(gameObject);
            return;
        }

        RotateTowardsPlayerFacing();

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndBlazingStar();
        }
    }

    void RotateTowardsPlayerFacing()
    {
        if (playerController == null)
            return;

        Vector2 dir = playerController.LastMoveDirection;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 225f;

        Quaternion targetRotation =
            Quaternion.Euler(0, 0, angle);

        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    void EndBlazingStar()
    {
        if (playerController != null)
        {
            playerController.StopForcedSpeed();
        }

        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
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
                enemy.transform.position - player.position;

            enemy.TakeDamage(
                (int)damage,
                dir,
                6f,
                0.2f
            );

            hitTimers[enemy] = time;
        }
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.maxSpeed = originalSpeed;
        }
    }
}