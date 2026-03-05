using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponData data;

    protected int currentDamage;
    protected float currentKnockback;
    protected float currentKnockbackDuration;
    protected float currentHitCooldown;

    public virtual void Initialize(WeaponData weaponData)
    {
        data = weaponData;

        currentDamage = data.baseDamage;
        currentKnockback = data.baseKnockback;
        currentKnockbackDuration = data.baseKnockbackDuration;
        currentHitCooldown = data.baseHitCooldown;
    }
}