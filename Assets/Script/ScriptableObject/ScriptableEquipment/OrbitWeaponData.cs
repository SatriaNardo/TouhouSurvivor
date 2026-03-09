using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Orbit Weapon")]
public class OrbitWeaponData : WeaponData
{
    [Header("Orbit")]
    public GameObject orbitPiecePrefab;
    public int weaponCount = 1;
    public float orbitDistance = 2f;
    public float rotateSpeed = 90f;

    [Header("Fade Settings")]
    public float fadeInTime = 0.3f;
    public float activeTime = 1f;
    public float fadeOutTime = 0.3f;
}