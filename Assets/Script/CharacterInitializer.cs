using UnityEngine;

public class CharacterInitializer : MonoBehaviour
{
    private CharacterData characterData;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private PlayerAttack playerAttack;
    private UltimateController ultimateController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAttack = GetComponent<PlayerAttack>();

        // Assuming UltimateController is on Canvas
        ultimateController = FindFirstObjectByType<UltimateController>();
    }

    public void Initialize(CharacterData data)
    {
        characterData = data;
        ApplyData();
    }

    void ApplyData()
    {
        if (characterData == null)
        {
            Debug.LogError("CharacterData is NULL!");
            return;
        }

        // Apply Sprite
        if (spriteRenderer != null)
            spriteRenderer.sprite = characterData.characterSprite;

        // Apply Movement Speed
        if (playerController != null)
            playerController.maxSpeed = characterData.moveSpeed;

        // Apply Starting Weapon
        if (playerAttack != null && characterData.startingWeapon != null)
            playerAttack.weaponManager.AddWeapon(characterData.startingWeapon);

      
        ultimateController.EquipUltimate(characterData.availableUltimates[0]);
    }
}