using UnityEngine;

public class Talisman : MonoBehaviour
{
    private Transform target;

    private ProjectileWeaponRuntimeStats stats;

    private Vector2 direction;

    private float homingDelay = 0.4f;
    private float homingTimer;
    private bool canHome = false;

    public void Initialize(Vector2 launchDirection, ProjectileWeaponRuntimeStats runtimeStats)
    {
        stats = runtimeStats;

        direction = launchDirection.normalized;

        homingTimer = homingDelay;
        canHome = false;
    }

    void Update()
    {
        if (stats == null)
            return;
        //Wait before homing
        if (!canHome)
        {
            homingTimer -= Time.deltaTime;

            if (homingTimer <= 0f)
                canHome = true;
        }
        //Homing
        if (canHome && stats.useHoming)
        {
            target = FindClosestEnemy();

            if (target != null)
            {
                Vector2 toTarget =
                    (target.position - transform.position);

                float distance = toTarget.magnitude;

                if (distance > 0.1f) // prevents orbit jitter
                {
                    Vector2 desiredDirection = toTarget.normalized;

                    // Controlled turn rate (prevents circling)
                    direction = Vector2.Lerp(
                        direction,
                        desiredDirection,
                        stats.homingStrength * Time.deltaTime
                    ).normalized;
                }
            }
        }
        // Movement
        transform.position +=
            (Vector3)(direction * stats.projectileSpeed * Time.deltaTime);
        // Rotate toward movement
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRot =
            Quaternion.Euler(0f, 0f, angle);

        transform.rotation =
            Quaternion.Lerp(transform.rotation, targetRot, 15f * Time.deltaTime);
    }
    Transform FindClosestEnemy()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float minDist = Mathf.Infinity;
        Enemy closest = null;

        foreach (Enemy e in enemies)
        {
            float dist =
                Vector2.Distance(transform.position,
                                 e.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest != null ? closest.transform : null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            Vector2 hitDir =
                (enemy.transform.position - transform.position).normalized;

            enemy.TakeDamage(
                stats.baseDamage,
                hitDir,
                stats.baseKnockback,
                stats.baseKnockbackDuration
            );

            Destroy(gameObject);
        }
    }
}