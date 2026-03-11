using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoBehaviour
{
    public Transform equipmentHolder;

    private List<Weapon> activeWeapons = new List<Weapon>();

    public void AddWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        if (weaponData is OrbitWeaponData orbitData)
        {
            SpawnOrbitWeapon(orbitData);
        }
        else if (weaponData is ProjectileWeaponData projectileData)
        {
            SpawnProjectileWeapon(projectileData);
        }
    }
    void SpawnOrbitWeapon(OrbitWeaponData orbitData)
    {
        GameObject controllerObj = Instantiate(
            orbitData.weaponPrefab,
            equipmentHolder
        );

        // VERY IMPORTANT
        controllerObj.transform.localPosition = Vector3.zero;
        controllerObj.transform.localRotation = Quaternion.identity;

        OrbitWeapon orbitWeapon =
            controllerObj.GetComponent<OrbitWeapon>();

        if (orbitWeapon != null)
        {
            orbitWeapon.Initialize(orbitData);
            activeWeapons.Add(orbitWeapon);
        }
    }

    void SpawnProjectileWeapon(ProjectileWeaponData projectileData)
    {
        GameObject weaponObj = Instantiate(
            projectileData.weaponPrefab,
            equipmentHolder
        );
        weaponObj.transform.localPosition = Vector3.zero;

        ProjectileWeapon projectileWeapon =
            weaponObj.GetComponent<ProjectileWeapon>();

        if (projectileWeapon != null)
        {
            projectileWeapon.Initialize(projectileData);
            activeWeapons.Add(projectileWeapon);
        }
    }
    public void ApplyToAllProjectiles(Action<ProjectileWeapon> upgrade)
    {
        foreach (var weapon in activeWeapons)
        {
            ProjectileWeapon projectile = weapon as ProjectileWeapon;

            if (projectile != null)
            {
                upgrade(projectile);
            }
        }
    }
}