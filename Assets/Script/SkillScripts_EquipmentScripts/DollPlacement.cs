using UnityEngine;
using System.Collections.Generic;

public class DollPlacement : MonoBehaviour, IOrbitPiece
{
    private OrbitWeaponRuntimeStats stats;
    private Transform player;

    private float orbitAngle;

    public float spinSpeed = 720f;

    private Dictionary<Enemy, float> hitTimers = new Dictionary<Enemy, float>();

    public void Initialize(OrbitWeaponRuntimeStats runtimeStats, Transform playerTransform, float startAngle)
    {
        stats = runtimeStats;
        player = playerTransform;
        orbitAngle = startAngle;
    }

    void Update()
    {
        if (stats == null || player == null) return;

        // ORBIT
        orbitAngle += stats.rotateSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        ) * stats.orbitDistance;

        transform.position = (Vector2)player.position + offset;

        // SPIN
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (stats == null) return;

        if (!other.TryGetComponent(out Enemy enemy))
            return;

        float currentTime = Time.time;

        if (!hitTimers.ContainsKey(enemy))
            hitTimers.Add(enemy, 0f);

        if (currentTime - hitTimers[enemy] >= stats.baseHitCooldown)
        {
            Vector2 dir =
                (enemy.transform.position - transform.position).normalized;

            enemy.TakeDamage(
                stats.baseDamage,
                dir,
                stats.baseKnockback,
                stats.baseKnockbackDuration
            );

            hitTimers[enemy] = currentTime;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (hitTimers.ContainsKey(enemy))
                hitTimers.Remove(enemy);
        }
    }
}