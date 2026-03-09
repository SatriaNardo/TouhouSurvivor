using UnityEngine;

public class FantasySealBehaviour : MonoBehaviour
{
    private float damage;
    private float explosionRadius;

    private float speed;
    private float maxSpeed;

    public float startSpeed = 1f;
    public float acceleration = 10f;

    public float homingStrength = 8f;

    public float spawnScale = 0.1f;
    public float scaleSpeed = 6f;

    private Transform target;
    private Vector2 direction;

    private float homingDelay = 0.4f;
    private float homingTimer;
    private bool canHome = false;

    private Vector3 targetScale;

    public void Initialize(float dmg, float radius, float moveSpeed, Vector2 launchDirection)
    {
        damage = dmg;
        explosionRadius = radius;

        maxSpeed = moveSpeed;
        speed = startSpeed;

        direction = launchDirection.normalized;

        homingTimer = homingDelay;
        canHome = false;

        targetScale = transform.localScale;
        transform.localScale = targetScale * spawnScale;
    }

    void Update()
    {
        speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);

        if (!canHome)
        {
            homingTimer -= Time.deltaTime;
            if (homingTimer <= 0f)
                canHome = true;
        }

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            scaleSpeed * Time.deltaTime
        );

        if (canHome)
        {
            if (target == null)
                target = FindClosestEnemy();

            if (target != null)
            {
                Vector2 toTarget = (target.position - transform.position);
                float distance = toTarget.magnitude;

                if (distance > 0.1f)
                {
                    Vector2 desiredDirection = toTarget.normalized;

                    direction = Vector2.Lerp(
                        direction,
                        desiredDirection,
                        homingStrength * Time.deltaTime
                    ).normalized;
                }
            }
        }

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 15f * Time.deltaTime);
    }

    Transform FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float minDist = Mathf.Infinity;
        Enemy closest = null;

        foreach (Enemy e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
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
            Explode();
        }
    }

    void Explode()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hitEnemies)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                Vector2 hitDir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage((int)damage, hitDir, 5f, 0.2f);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}