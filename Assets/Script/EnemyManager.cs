using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    [HideInInspector] public Transform player;

    [Header("Spawn Settings")]
    public float spawnInterval = 1f;
    public float spawnOffset = 2f;

    [Header("Enemy Settings")]
    public float enemySpeed = 2f;

    private Camera cam;
    private float spawnTimer;
    private List<Rigidbody2D> activeEnemies = new List<Rigidbody2D>();

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleSpawning();
        HandleEnemyMovement();
        CleanupDestroyedEnemies();
    }

    void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector2 spawnPos = GetSpawnPositionOutsideCamera();
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = enemyObj.GetComponent<Rigidbody2D>();
        if (rb != null)
            activeEnemies.Add(rb);
    }

    Vector2 GetSpawnPositionOutsideCamera()
    {
        if (cam == null) return Vector2.zero;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector2 center = cam.transform.position;
        int side = Random.Range(0, 4);

        Vector2 offset = Vector2.zero;

        switch (side)
        {
            case 0:
                offset = new Vector2(Random.Range(-camWidth, camWidth), camHeight + spawnOffset);
                break;
            case 1:
                offset = new Vector2(Random.Range(-camWidth, camWidth), -camHeight - spawnOffset);
                break;
            case 2:
                offset = new Vector2(-camWidth - spawnOffset, Random.Range(-camHeight, camHeight));
                break;
            case 3:
                offset = new Vector2(camWidth + spawnOffset, Random.Range(-camHeight, camHeight));
                break;
        }

        return center + offset;
    }

    void HandleEnemyMovement()
    {
        if (player == null) return;

        foreach (var rb in activeEnemies)
        {
            if (rb == null) continue;

            Enemy enemy = rb.GetComponent<Enemy>();

            if (enemy != null && enemy.IsKnockedBack)
            {
                rb.linearVelocity = Vector2.zero;
                continue;
            }

            Vector2 direction = (player.position - rb.transform.position).normalized;
            rb.linearVelocity = direction * enemySpeed;
        }
    }
    void CleanupDestroyedEnemies()
    {
        activeEnemies.RemoveAll(rb => rb == null);
    }
    // ================= ACCESS FOR WEAPONS =================
    public List<Rigidbody2D> GetActiveEnemies()
    {
        return activeEnemies;
    }
}