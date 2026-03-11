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

    private Vector2 lastMoveDirection = Vector2.right;

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
        if (runtimeStats == null)
            return;

        if (player != null && player.MoveDirection != Vector2.zero)
        {
            lastMoveDirection = player.MoveDirection.normalized;
        }

        if (isAttacking)
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

        if (runtimeStats.requireEnemyToShoot && target == null)
        {
            attackTimer = runtimeStats.attackInterval;
            isAttacking = false;
            yield break;
        }

        // Otherwise fire normally
        yield return StartCoroutine(FireWaves());

        attackTimer = runtimeStats.attackInterval;
        isAttacking = false;
    }

    // ================= WAVES =================

    IEnumerator FireWaves()
    {       
    if (player == null)
        yield break;

    Vector2 baseDir = lastMoveDirection;

    if (runtimeStats.targetEnemy)
    {
        Enemy target = FindClosestEnemyOnScreen();

        if (target != null)
        {
            baseDir =
                (target.transform.position - transform.position).normalized;
        }
    }

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

        for (int b = 0; b < bursts; b++)
        {
            // Random angle inside cone
            float randomAngle = Random.insideUnitCircle.x * cone * 0.5f;

            Vector2 rotatedBase =
                Quaternion.Euler(0f, 0f, randomAngle) * baseDir;

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

        // Try Talisman
        Talisman talisman = projObj.GetComponent<Talisman>();
        if (talisman != null)
        {
            talisman.Initialize(direction, runtimeStats);
            return;
        }

        // Try MeteoricDebris
        MeteoricDebris debris = projObj.GetComponent<MeteoricDebris>();
        if (debris != null)
        {
            debris.Initialize(direction, runtimeStats);
            return;
        }
        
        // Try SilverKnives
        SilverKnife knife = projObj.GetComponent<SilverKnife>();
        if (knife != null)
        {
            knife.Initialize(direction, runtimeStats);
            return;
        }

        // Safety warning
        Debug.LogWarning(
            "Projectile prefab has no supported projectile script attached."
        );
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



    // ================= GLOBAL UPGRADE FUNCTIONS =================

    // DAMAGE
    public void UpgradeDamagePercent(float percent)
    {
        runtimeStats.baseDamage =
            Mathf.RoundToInt(runtimeStats.baseDamage * (1f + percent));
    }

    public void UpgradeDamageFlat(int amount)
    {
        runtimeStats.baseDamage += amount;
    }


    // KNOCKBACK
    public void UpgradeKnockback(float amount)
    {
        runtimeStats.baseKnockback += amount;
    }

    public void UpgradeKnockbackDuration(float amount)
    {
        runtimeStats.baseKnockbackDuration += amount;
    }


    // HIT COOLDOWN
    public void UpgradeHitCooldown(float percent)
    {
        runtimeStats.baseHitCooldown *= (1f - percent);
    }


    // ATTACK SPEED
    public void UpgradeAttackSpeed(float percent)
    {
        runtimeStats.attackInterval *= (1f - percent);
    }


    // PROJECTILE SPEED
    public void UpgradeProjectileSpeed(float percent)
    {
        runtimeStats.projectileSpeed *= (1f + percent);
    }

    public void UpgradeProjectileSpeedFlat(float amount)
    {
        runtimeStats.projectileSpeed += amount;
    }


    // SPREAD
    public void UpgradeSpread(int amount)
    {
        runtimeStats.spreadCount += amount;
    }


    // CONE ANGLE
    public void UpgradeConeAngle(float amount)
    {
        runtimeStats.coneAngle += amount;
    }


    // BURST
    public void UpgradeBurst(int amount)
    {
        runtimeStats.burstCount += amount;
    }

    public void UpgradeBurstDelay(float percent)
    {
        runtimeStats.delayBetweenBursts *= (1f - percent);
    }


    // WAVE
    public void UpgradeWaveCount(int amount)
    {
        runtimeStats.waveCount += amount;
    }

    public void UpgradeWaveInterval(float percent)
    {
        runtimeStats.waveInterval *= (1f - percent);
    }


    // HOMING
    public void UpgradeHomingStrength(float amount)
    {
        runtimeStats.homingStrength += amount;
    }
}