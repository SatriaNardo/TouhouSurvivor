using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public WeaponManager weaponManager;

    public void Initialize(CharacterData data)
    {
        if (data == null || data.startingWeapon == null)
            return;

        weaponManager.AddWeapon(data.startingWeapon);
    }
}