[System.Serializable]
public class ProjectileWeaponRuntimeStats
{
    public int baseDamage;
    public float baseKnockback;
    public float baseKnockbackDuration;
    public float baseHitCooldown;

    public float attackInterval;
    public float projectileSpeed;

    public int spreadCount;
    public float coneAngle;

    public int burstCount;
    public float delayBetweenBursts;

    public int waveCount;
    public float waveInterval;

    public bool useHoming;
    public float homingStrength;

    public bool requireEnemyToShoot; 

    public ProjectileWeaponRuntimeStats(ProjectileWeaponData data)
    {
        baseDamage = data.baseDamage;
        baseKnockback = data.baseKnockback;
        baseKnockbackDuration = data.baseKnockbackDuration;
        baseHitCooldown = data.baseHitCooldown;

        attackInterval = data.attackInterval;
        projectileSpeed = data.projectileSpeed;

        spreadCount = data.spreadCount;
        coneAngle = data.coneAngle;

        burstCount = data.burstCount;
        delayBetweenBursts = data.delayBetweenBursts;

        waveCount = data.waveCount;
        waveInterval = data.waveInterval;

        useHoming = data.useHoming;
        homingStrength = data.homingStrength;

        requireEnemyToShoot = data.requireEnemyToShoot; 
    }
}