using UnityEngine;
using System.Collections.Generic;

public class OrbitWeapon : Weapon
{
    protected OrbitWeaponData orbitData;

    [SerializeField]
    protected OrbitWeaponRuntimeStats runtimeStats;

    protected Dictionary<Enemy, float> hitTimers =
        new Dictionary<Enemy, float>();

    private List<Transform> orbitInstances =
        new List<Transform>();

    private int lastWeaponCount;

    public virtual void Initialize(OrbitWeaponData weaponData)
    {
        base.Initialize(weaponData);

        orbitData = weaponData;
        runtimeStats = new OrbitWeaponRuntimeStats(orbitData);

        lastWeaponCount = runtimeStats.weaponCount;

        BuildOrbit();
    }

    protected virtual void Update()
    {
        if (runtimeStats == null)
            return;

        transform.Rotate(
            Vector3.forward *
            runtimeStats.rotateSpeed *
            Time.deltaTime
        );

        if (runtimeStats.weaponCount != lastWeaponCount)
        {
            RebuildOrbit();
        }
    }

    void BuildOrbit()
    {
        orbitInstances.Clear();

        for (int i = 0; i < runtimeStats.weaponCount; i++)
        {
            GameObject obj = Instantiate(
                orbitData.orbitPiecePrefab,
                transform
            );

            obj.transform.localPosition = Vector3.zero;

            NonDirectionalLaser piece = obj.GetComponent<NonDirectionalLaser>();

            if (piece != null)
            {
                piece.SetController(this);
                piece.SetFadeStats(
                    runtimeStats.fadeInTime,
                    runtimeStats.activeTime,
                    runtimeStats.fadeOutTime
                );
            }

            orbitInstances.Add(obj.transform);
        }

        ArrangeOrbit();
    }

    void RebuildOrbit()
    {
        lastWeaponCount = runtimeStats.weaponCount;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        orbitInstances.Clear();

        BuildOrbit();
    }

    void ArrangeOrbit()
    {
        if (runtimeStats.weaponCount <= 0)
            return;

        float angleStep = 360f / runtimeStats.weaponCount;

        for (int i = 0; i < orbitInstances.Count; i++)
        {
            float angle = angleStep * i;

            Vector3 offset =
                Quaternion.Euler(0, 0, angle) *
                Vector3.right *
                runtimeStats.orbitDistance;

            orbitInstances[i].localPosition = offset;
            orbitInstances[i].localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // ================= DAMAGE =================

    public void HandleDamage(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
            return;

        float currentTime = Time.time;

        if (!hitTimers.ContainsKey(enemy))
            hitTimers.Add(enemy, 0f);

        if (currentTime - hitTimers[enemy]
            >= runtimeStats.baseHitCooldown)
        {
            Vector2 hitDirection =
                (enemy.transform.position -
                 transform.position);

            enemy.TakeDamage(
                runtimeStats.baseDamage,
                hitDirection,
                runtimeStats.baseKnockback,
                runtimeStats.baseKnockbackDuration
            );

            hitTimers[enemy] = currentTime;
        }
    }

    public void HandleExit(Collider2D other)
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

    // ================= UPGRADES =================

    public void UpgradeDamage(float percent)
    {
        runtimeStats.baseDamage =
            Mathf.RoundToInt(
                runtimeStats.baseDamage *
                (1f + percent)
            );
    }

    public void UpgradeRotateSpeed(float percent)
    {
        runtimeStats.rotateSpeed *= (1f + percent);
    }

    public void UpgradeOrbitDistance(float amount)
    {
        runtimeStats.orbitDistance += amount;
        ArrangeOrbit();
    }

    public void UpgradeWeaponCount(int amount)
    {
        runtimeStats.weaponCount += amount;
    }

    public void UpgradeFadeSpeed(float percent)
    {
        runtimeStats.fadeInTime *= (1f - percent);
        runtimeStats.fadeOutTime *= (1f - percent);

        RefreshFadeStats();
    }

    public void UpgradeActiveTime(float percent)
    {
        runtimeStats.activeTime *= (1f + percent);
        RefreshFadeStats();
    }

    void RefreshFadeStats()
    {
        foreach (Transform t in orbitInstances)
        {
            NonDirectionalLaser piece = t.GetComponent<NonDirectionalLaser>();

            if (piece != null)
            {
                piece.SetFadeStats(
                    runtimeStats.fadeInTime,
                    runtimeStats.activeTime,
                    runtimeStats.fadeOutTime
                );
            }
        }
    }
}