using UnityEngine;

public class SilverKnife : MonoBehaviour
{
    private ProjectileWeaponRuntimeStats stats;

    private Vector2 direction;
    private float speed;
    private int damage;

    private Camera mainCam;

    private int remainingBounces;

    private float minX, maxX, minY, maxY;

    private float padding = 0.05f;

    public void Initialize(Vector2 dir, ProjectileWeaponRuntimeStats runtimeStats)
    {
        stats = runtimeStats;

        direction = dir.normalized;
        speed = stats.projectileSpeed;
        damage = stats.baseDamage;

        mainCam = Camera.main;

        remainingBounces = stats.bounceCount;

        CalculateCameraBounds();

        RotateToDirection();
    }

    void Update()
    {
        if (SakuyaWorldBehaviour.Instance != null &&
            SakuyaWorldBehaviour.Instance.IsTimeStopped)
            return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        CheckScreenCollision();
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

    void CalculateCameraBounds()
    {
        float height = mainCam.orthographicSize * 2f;
        float width = height * mainCam.aspect;

        Vector3 camPos = mainCam.transform.position;

        minX = camPos.x - width / 2f;
        maxX = camPos.x + width / 2f;

        minY = camPos.y - height / 2f;
        maxY = camPos.y + height / 2f;
    }

    void CheckScreenCollision()
    {
        if (mainCam == null) return;

        CalculateCameraBounds();

        Vector3 pos = transform.position;
        bool bounced = false;

        // LEFT
        if (pos.x <= minX + padding)
        {
            if (stats.bounceScreen && remainingBounces > 0)
            {
                pos.x = minX + padding;
                direction.x = Mathf.Abs(direction.x);
                bounced = true;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // RIGHT
        else if (pos.x >= maxX - padding)
        {
            if (stats.bounceScreen && remainingBounces > 0)
            {
                pos.x = maxX - padding;
                direction.x = -Mathf.Abs(direction.x);
                bounced = true;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // BOTTOM
        if (pos.y <= minY + padding)
        {
            if (stats.bounceScreen && remainingBounces > 0)
            {
                pos.y = minY + padding;
                direction.y = Mathf.Abs(direction.y);
                bounced = true;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // TOP
        else if (pos.y >= maxY - padding)
        {
            if (stats.bounceScreen && remainingBounces > 0)
            {
                pos.y = maxY - padding;
                direction.y = -Mathf.Abs(direction.y);
                bounced = true;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        if (bounced)
        {
            remainingBounces--;

            transform.position = pos;

            direction.Normalize();

            RotateToDirection();
        }
    }

    void RotateToDirection()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}