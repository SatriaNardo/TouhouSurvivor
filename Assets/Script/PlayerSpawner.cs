using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public CharacterData choosenCharacter;

    [Header("Scene References")]
    public FollowCamera followCamera;
    public EnemyManager enemyManager;

    void Start()
    {
        SelectCharacter(choosenCharacter);
        SpawnPlayer();
    }

    public void SelectCharacter(CharacterData data)
    {
        CharacterSelectionManager.SelectedCharacter = data;
    }

    void SpawnPlayer()
    {
        if (CharacterSelectionManager.SelectedCharacter == null)
        {
            Debug.LogError("No character selected!");
            return;
        }

        GameObject player = Instantiate(
            playerPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // Initialize character stats
        CharacterInitializer initializer = player.GetComponent<CharacterInitializer>();
        if (initializer != null)
        {
            initializer.Initialize(CharacterSelectionManager.SelectedCharacter);
        }

        Transform playerTransform = player.transform;

        // Assign Camera target
        if (followCamera != null)
            followCamera.target = playerTransform;

        // Assign Enemy target
        if (enemyManager != null)
            enemyManager.player = playerTransform;

        WeaponManager weaponManager = player.GetComponent<WeaponManager>();

        WeaponAdd weaponAddUI = FindFirstObjectByType<WeaponAdd>();

        if (weaponManager != null && weaponAddUI != null)
        {
            weaponAddUI.managerWeapons = weaponManager;
        }
    }
}