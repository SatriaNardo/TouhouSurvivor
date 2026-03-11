using UnityEngine;

public class SilverKnife : MonoBehaviour
{
    private ProjectileWeaponRuntimeStats stats;

    private Vector2 direction;
    private float speed;
    private int damage;

    private Camera mainCam;
    private Enemy target;

    public void Initialize(Vector2 dir, ProjectileWeaponRuntimeStats runtimeStats)
    {
        direction = dir.normalized;
        stats = runtimeStats;

        speed = stats.projectileSpeed;
        damage = stats.baseDamage;

        mainCam = Camera.main;

        // rotate sprite toward movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        CheckOffScreen();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (stats == null) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            Vector2 hitDir = direction;

            enemy.TakeDamage(damage, hitDir, 6f, 0.2f);

            if (!stats.pierceEnemy)
            {
                Destroy(gameObject);
            }
        }
    }

    void CheckOffScreen()
    {
        if (mainCam == null) return;

        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);

        bool outside =
            viewPos.x < -0.1f ||
            viewPos.x > 1.1f ||
            viewPos.y < -0.1f ||
            viewPos.y > 1.1f;

        if (outside)
        {
            Destroy(gameObject);
        }
    }
}