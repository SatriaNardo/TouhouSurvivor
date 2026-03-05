using UnityEngine;

[System.Serializable]
public class ProjectileWeaponRuntimeStats
{
    // ================= BASE STATS =================
    public int baseDamage;
    public float baseKnockback;
    public float baseKnockbackDuration;
    public float baseHitCooldown;

    // ================= ATTACK =================
    public float attackInterval;
    public float projectileSpeed;

    // ================= SPREAD =================
    public int spreadCount;
    public float coneAngle;

    // ================= BURST =================
    public int burstCount;
    public float delayBetweenBursts;

    // ================= WAVES =================
    public int waveCount;
    public float waveInterval;

    // ================= HOMING =================
    public bool useHoming;
    public float homingStrength;

    // ================= CONSTRUCTOR =================
    public ProjectileWeaponRuntimeStats(ProjectileWeaponData data)
    {
        // Base
        baseDamage = data.baseDamage;
        baseKnockback = data.baseKnockback;
        baseKnockbackDuration = data.baseKnockbackDuration;
        baseHitCooldown = data.baseHitCooldown;

        // Attack
        attackInterval = data.attackInterval;
        projectileSpeed = data.projectileSpeed;

        // Spread
        spreadCount = data.spreadCount;
        coneAngle = data.coneAngle;

        // Burst
        burstCount = data.burstCount;
        delayBetweenBursts = data.delayBetweenBursts;

        // Waves
        waveCount = data.waveCount;
        waveInterval = data.waveInterval;

        // Homing
        useHoming = data.useHoming;
        homingStrength = data.homingStrength;
    }
}