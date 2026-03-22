using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantasmicKillerBehavior : MonoBehaviour
{
    private PhantasmicKiller data;
    private GameObject user;

    public void Activate(PhantasmicKiller data, GameObject user)
    {
        this.data = data;
        this.user = user;

        StartCoroutine(SpawnKnives());
    }

    private IEnumerator SpawnKnives()
    {
        for (int i = 0; i < data.totalKnives; i++)
        {
            SpawnKnife();
            yield return new WaitForSeconds(data.spawnRate);
        }
    }

    private void SpawnKnife()
    {
        GameObject knife = Instantiate(
            data.knifePrefab,
            user.transform.position,
            Quaternion.identity
        );

        KnifeRuntime runtime = knife.AddComponent<KnifeRuntime>();
        runtime.Init(data.speed, data.damage, data.bounceCount, data.lifetime);
    }

    // =========================================================
    // INNER CLASS – screen bounce + pierce + impact damage
    // =========================================================
    public class KnifeRuntime : MonoBehaviour
    {
        private float speed;
        private float damage;
        private int remainingBounces;
        private float lifetime;

        private Vector2 direction;

        private Camera mainCam;

        private float minX, maxX, minY, maxY;
        private float padding = 0.05f;

        private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

        public void Init(float speed, float damage, int bounce, float lifetime)
        {
            this.speed = speed;
            this.damage = damage;
            this.remainingBounces = bounce;
            this.lifetime = lifetime;

            direction = Random.insideUnitCircle.normalized;

            mainCam = Camera.main;

            CalculateCameraBounds();
            RotateToDirection();

            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            CheckScreenCollision();
        }

        // ================= SCREEN BOUNCE =================
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
                if (remainingBounces > 0)
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
                if (remainingBounces > 0)
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
                if (remainingBounces > 0)
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
                if (remainingBounces > 0)
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

                // OPTIONAL: allow re-hit after bounce
                hitEnemies.Clear();
            }
        }

        // ================= DAMAGE (PIERCE) =================
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy"))
                return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null)
                return;

            if (hitEnemies.Contains(enemy))
                return;

            hitEnemies.Add(enemy);

            Vector2 hitDir = direction;

            enemy.TakeDamage(
                (int)damage,
                hitDir,
                5f,
                0.2f
            );
        }

        // ================= ROTATION =================
        void RotateToDirection()
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}