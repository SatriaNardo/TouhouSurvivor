using UnityEngine;

public abstract class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public int baseDamage = 1;
    public float baseKnockback = 5f;
    public float baseKnockbackDuration = 0.15f;
    public float baseHitCooldown = 0.3f;
}