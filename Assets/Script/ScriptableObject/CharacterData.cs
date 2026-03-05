using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Game/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;

    public float moveSpeed = 5f;

    public WeaponData startingWeapon;

    public UltimateData[] availableUltimates;

    [Header("Default Equipped Ultimate")]
    public UltimateData selectedUltimate;
}