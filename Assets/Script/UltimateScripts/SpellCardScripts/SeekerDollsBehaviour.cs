using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerDollsBehavior : MonoBehaviour
{
    private SeekerDolls data;
    private Transform player;

    private List<GameObject> circles = new List<GameObject>();

    public void Activate(SeekerDolls data, GameObject user)
    {
        this.data = data;
        this.player = user.transform;

        SpawnCircles();
        StartCoroutine(Lifetime());
    }

    void SpawnCircles()
    {
        for (int i = 0; i < data.circleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / data.circleCount;

            Vector2 offset = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * data.radius;

            GameObject circle = Instantiate(
                data.circlePrefab,
                player.position + (Vector3)offset,
                Quaternion.identity
            );

            circle.transform.SetParent(player);

            SeekerDollCircle circleLogic =
                circle.AddComponent<SeekerDollCircle>();

            circleLogic.Init(data, angle * Mathf.Rad2Deg);

            circles.Add(circle);
        }
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(data.duration);

        foreach (var c in circles)
        {
            if (c != null) Destroy(c);
        }

        Destroy(this);
    }

    // =====================================================
    // 🔵 CIRCLE (EMITTER)
    // =====================================================
    public class SeekerDollCircle : MonoBehaviour
    {
        private SeekerDolls data;

        private float baseAngle;
        private float currentAngle;
        private float direction = 1f;

        private float halfCone;

        private GameObject laserInstance;

        public void Init(SeekerDolls data, float angle)
        {
            this.data = data;

            baseAngle = angle;
            currentAngle = angle;

            halfCone = data.coneAngle / 2f;

            SpawnLaser();
        }

        void SpawnLaser()
        {
            if (data.laserPrefab == null)
            {
                Debug.LogError("Laser Prefab missing!");
                return;
            }
            
            float offsetDistance = 0.25f;

            laserInstance = Instantiate(
                data.laserPrefab,
                transform.position + transform.right * offsetDistance,
                Quaternion.Euler(0, 0, 90f),
                transform
            );

            SeekerDollLaser laser =
                laserInstance.AddComponent<SeekerDollLaser>();

            laser.Init(data);
        }

        void Update()
        {
            Sweep();
        }

        void Sweep()
        {
            currentAngle += direction * data.sweepSpeed * Time.deltaTime;

            float min = baseAngle - halfCone;
            float max = baseAngle + halfCone;

            if (currentAngle > max)
            {
                currentAngle = max;
                direction *= -1f;
            }
            else if (currentAngle < min)
            {
                currentAngle = min;
                direction *= -1f;
            }

            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    // =====================================================
    // 🔥 LASER (DAMAGE)
    // =====================================================
   public class SeekerDollLaser : MonoBehaviour
    {
        private SeekerDolls data;

        private Dictionary<Enemy, float> hitTimers =
            new Dictionary<Enemy, float>();

        public void Init(SeekerDolls data)
        {
            this.data = data;
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy"))
                return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;

            float time = Time.time;

            if (!hitTimers.ContainsKey(enemy))
                hitTimers.Add(enemy, 0f);

            if (time - hitTimers[enemy] >= data.hitCooldown)
            {
                Vector2 dir = transform.right;

                enemy.TakeDamage(
                    (int)data.damage,
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

            if (enemy != null && hitTimers.ContainsKey(enemy))
            {
                hitTimers.Remove(enemy);
            }
        }
    }
}