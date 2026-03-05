using System.Collections;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    private ProjectileWeaponData projectileData;

    [SerializeField]
    private ProjectileWeaponRuntimeStats runtimeStats;

    private float attackTimer;

    private EnemyManager enemyManager;
    private Camera mainCam;

    private bool isAttacking = false;

    private PlayerController player;

    // ================= INITIALIZE =================

    public void Initialize(ProjectileWeaponData weaponData)
    {
        base.Initialize(weaponData);

        projectileData = weaponData;
        runtimeStats = new ProjectileWeaponRuntimeStats(projectileData);

        enemyManager = FindFirstObjectByType<EnemyManager>();
        player = FindFirstObjectByType<PlayerController>();
        mainCam = Camera.main;

        attackTimer = runtimeStats.attackInterval;
    }

    // ================= UPDATE =================

    void Update()
    {
        if (runtimeStats == null || isAttacking)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // ================= MAIN ATTACK =================

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        Enemy target = FindClosestEnemyOnScreen();

        if (target != null)
        {
            yield return StartCoroutine(FireWaves());
        }

        attackTimer = runtimeStats.attackInterval;
        isAttacking = false;
    }

    // ================= WAVES =================

    IEnumerator FireWaves()
    {
        if (player == null)
            yield break;

        Vector2 baseDir = player.MoveDirection;

        if (baseDir == Vector2.zero)
            baseDir = Vector2.right;

        for (int w = 0; w < runtimeStats.waveCount; w++)
        {
            yield return StartCoroutine(FireBurst(baseDir));

            if (w < runtimeStats.waveCount - 1)
                yield return new WaitForSeconds(runtimeStats.waveInterval);
        }
    }

    // ================= BURST =================

    IEnumerator FireBurst(Vector2 baseDir)
    {
        int bursts = runtimeStats.burstCount;
        float cone = runtimeStats.coneAngle;

        float burstStep = 0f;

        if (bursts > 1)
            burstStep = cone / (bursts - 1);

        float startAngle = -cone * 0.5f;

        for (int b = 0; b < bursts; b++)
        {
            float burstAngle = startAngle + burstStep * b;

            Vector2 rotatedBase =
                Quaternion.Euler(0f, 0f, burstAngle) * baseDir;

            FireSpread(rotatedBase);

            if (b < bursts - 1)
                yield return new WaitForSeconds(runtimeStats.delayBetweenBursts);
        }
    }
    // ================= SPREAD =================

    void FireSpread(Vector2 baseDir)
    {
        int spread = runtimeStats.spreadCount;

        if (spread <= 1)
        {
            SpawnProjectile(baseDir);
            return;
        }

        // Divide total cone across spread layers
        float internalCone = runtimeStats.coneAngle / runtimeStats.burstCount;

        float angleStep = internalCone / (spread - 1);
        float startAngle = -internalCone * 0.5f;

        for (int i = 0; i < spread; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 rotatedDir =
                Quaternion.Euler(0f, 0f, angle) * baseDir;

            SpawnProjectile(rotatedDir.normalized);
        }
    }

    // ================= SPAWN =================

    void SpawnProjectile(Vector2 direction)
    {
        GameObject projObj = Instantiate(
            projectileData.projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Talisman projectile =
            projObj.GetComponent<Talisman>();

        if (projectile != null)
        {
            projectile.Initialize(direction, runtimeStats);
        }
    }

    // ================= TARGETING =================

    Enemy FindClosestEnemyOnScreen()
    {
        if (enemyManager == null || mainCam == null)
            return null;

        float minDist = Mathf.Infinity;
        Enemy closest = null;

        foreach (var rb in enemyManager.GetActiveEnemies())
        {
            if (rb == null) continue;

            Enemy enemy = rb.GetComponent<Enemy>();
            if (enemy == null) continue;

            Vector3 viewportPos =
                mainCam.WorldToViewportPoint(enemy.transform.position);

            bool isVisible =
                viewportPos.x >= 0 && viewportPos.x <= 1 &&
                viewportPos.y >= 0 && viewportPos.y <= 1 &&
                viewportPos.z > 0;

            if (!isVisible) continue;

            float dist =
                Vector2.Distance(transform.position,
                                 enemy.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    // ================= UPGRADE FUNCTIONS =================

    public void UpgradeDamage(float percent)
    {
        runtimeStats.baseDamage =
            Mathf.RoundToInt(runtimeStats.baseDamage * (1f + percent));
    }

    public void UpgradeAttackSpeed(float percent)
    {
        runtimeStats.attackInterval *= (1f - percent);
    }

    public void UpgradeSpread(int amount)
    {
        runtimeStats.spreadCount += amount;
    }

    public void UpgradeBurst(int amount)
    {
        runtimeStats.burstCount += amount;
    }

    public void UpgradeCone(float extraAngle)
    {
        runtimeStats.coneAngle += extraAngle;
    }

    public void UpgradeWaveCount(int amount)
    {
        runtimeStats.waveCount += amount;
    }

    public void UpgradeProjectileSpeed(float percent)
    {
        runtimeStats.projectileSpeed *= (1f + percent);
    }

    public void UpgradeHoming(float extraStrength)
    {
        runtimeStats.homingStrength += extraStrength;
    }
}