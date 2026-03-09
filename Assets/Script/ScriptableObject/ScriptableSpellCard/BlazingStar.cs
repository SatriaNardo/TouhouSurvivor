using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Blazing Star")]
public class BlazingStar : UltimateData
{
    public GameObject blazingStarPrefab;

    public float duration = 4f;
    public float damage = 40f;
    public float hitCooldown = 0.25f;

    public float speedMultiplier = 3f;

    public override void Activate(GameObject user)
    {
        if (blazingStarPrefab == null || user == null)
            return;

        UltimateController controller = Object.FindFirstObjectByType<UltimateController>();
        UltimateRuntimeStats stats = controller.GetRuntimeStats(this);

        float finalDamage =
            UltimateStatCalculator.GetStat(
                damage,
                UltimateUpgradeType.Damage,
                stats
            );

        float finalDuration =
            UltimateStatCalculator.GetStat(
                duration,
                UltimateUpgradeType.Duration,
                stats
            );

        float finalSpeed =
            UltimateStatCalculator.GetStat(
                speedMultiplier,
                UltimateUpgradeType.SpeedMultiplier,
                stats
            );

        GameObject obj = Instantiate(
            blazingStarPrefab,
            user.transform.position,
            Quaternion.identity,
            user.transform
        );

        BlazingStarBehaviour blazingStar =
            obj.GetComponent<BlazingStarBehaviour>();

        if (blazingStar != null)
        {
            blazingStar.Initialize(
                user.transform,
                finalDuration,
                finalDamage,
                hitCooldown,
                finalSpeed
            );
        }
    }
}