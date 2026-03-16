using UnityEngine;

public class MeteoricDebris : MonoBehaviour
{
    private ProjectileWeaponRuntimeStats stats;

    private Vector2 direction;

    private Camera mainCam;

    public void Initialize(Vector2 launchDirection, ProjectileWeaponRuntimeStats runtimeStats)
    {
        stats = runtimeStats;

        // If direction is zero, fallback to right
        if (launchDirection == Vector2.zero)
            launchDirection = Vector2.right;

        direction = launchDirection.normalized;

        mainCam = Camera.main;
    }

    void Update()
    {
        if (stats == null)
            return;
        if (SakuyaWorldBehaviour.Instance != null &&
            SakuyaWorldBehaviour.Instance.IsTimeStopped)
            return;
        // Move forward
        transform.position +=
            (Vector3)(direction * stats.projectileSpeed * Time.deltaTime);

        // Rotate toward movement
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRot =
            Quaternion.Euler(0f, 0f, angle);

        transform.rotation =
            Quaternion.Lerp(transform.rotation, targetRot, 10f * Time.deltaTime);

        // Spin meteor
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);

        // Remove when outside screen
        CheckIfOffScreen();
    }

    void CheckIfOffScreen()
    {
        if (mainCam == null) return;

        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);

        if (viewPos.x < -0.2f || viewPos.x > 1.2f ||
            viewPos.y < -0.2f || viewPos.y > 1.2f)
        {
            Destroy(gameObject);
        }
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