using UnityEngine;

public class WeaponAdd : MonoBehaviour
{
    [HideInInspector] public WeaponManager managerWeapons;

    public void AddingWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;
        if (managerWeapons == null)
        {
            Debug.LogError("WeaponManager not assigned!");
            return;
        }

        managerWeapons.AddWeapon(weaponData);
    }
    public void UpgradeStuff()
    {
        managerWeapons.ApplyToAllProjectiles(w => w.UpgradeBurst(2));
    }
}