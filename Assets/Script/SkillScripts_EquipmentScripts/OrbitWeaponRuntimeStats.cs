using UnityEngine;

[System.Serializable]
public class OrbitWeaponRuntimeStats
{
    public int baseDamage;
    public float baseKnockback;
    public float baseKnockbackDuration;
    public float baseHitCooldown;

    public float rotateSpeed;
    public float orbitDistance;
    public int weaponCount;

    public float fadeInTime;
    public float activeTime;
    public float fadeOutTime;

    public OrbitWeaponRuntimeStats(OrbitWeaponData data)
    {
        baseDamage = data.baseDamage;
        baseKnockback = data.baseKnockback;
        baseKnockbackDuration = data.baseKnockbackDuration;
        baseHitCooldown = data.baseHitCooldown;

        rotateSpeed = data.rotateSpeed;
        orbitDistance = data.orbitDistance;
        weaponCount = data.weaponCount;

        fadeInTime = data.fadeInTime;
        activeTime = data.activeTime;
        fadeOutTime = data.fadeOutTime;
    }
}