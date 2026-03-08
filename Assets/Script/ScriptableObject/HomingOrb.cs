using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Ultimate/Homing Orbs")]
public class HomingOrb : UltimateData
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public int orbCount = 3;
    public float spawnDelay = 0.2f;

    [Header("Stats")]
    public float damage = 40f;
    public float explosionRadius = 2.5f;
    public float moveSpeed = 8f;

    public override void Activate(GameObject user)
    {
        if (orbPrefab == null || user == null) return;

        UltimateController controller = Object.FindFirstObjectByType<UltimateController>();
        UltimateRuntimeStats stats = controller.GetRuntimeStats(this);

        int finalOrbCount =
            (int)UltimateStatCalculator.GetStat(
                orbCount,
                UltimateUpgradeType.OrbCount,
                stats
            );

        float finalDamage =
            UltimateStatCalculator.GetStat(
                damage,
                UltimateUpgradeType.Damage,
                stats
            );

        float finalRadius =
            UltimateStatCalculator.GetStat(
                explosionRadius,
                UltimateUpgradeType.ExplosionRadius,
                stats
                );

        user.GetComponent<MonoBehaviour>().StartCoroutine(
            SpawnOrbs(user, finalOrbCount, finalDamage, finalRadius)
        );
    }

    IEnumerator SpawnOrbs(GameObject user, int count, float dmg, float radius)
    {
        Vector2 launchDir = user.transform.right;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = user.transform.position + (Vector3)(launchDir * 0.5f);

            GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

            HomingOrbBehaviour orbBehaviour = orb.GetComponent<HomingOrbBehaviour>();
            if (orbBehaviour != null)
            {
                orbBehaviour.Initialize(dmg, radius, moveSpeed, launchDir);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}