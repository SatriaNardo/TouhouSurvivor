using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Projectile Weapon")]
public class ProjectileWeaponData : WeaponData
{
    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Attack")]
    public float attackInterval = 1f;
    public float projectileSpeed = 8f;

    [Header("Spread (Simultaneous)")]
    public int spreadCount = 1;
    public float coneAngle = 30f;

    [Header("Burst (Sequential)")]
    public int burstCount = 1;
    public float delayBetweenBursts = 0.08f;

    [Header("Waves")]
    public int waveCount = 1;
    public float waveInterval = 0.15f;

    [Header("Homing")]
    public bool useHoming = false;
    public float homingStrength = 5f;
}