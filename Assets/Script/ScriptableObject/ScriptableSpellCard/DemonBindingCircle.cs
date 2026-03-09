using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Demon Binding Circle")]
public class DemonBindingCircle : UltimateData
{
    public GameObject circlePrefab;

    public float duration = 2f;
    public float damage = 80f;

    public override void Activate(GameObject user)
    {
        if (circlePrefab == null || user == null)
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

        GameObject obj = Instantiate(
            circlePrefab,
            user.transform.position,
            Quaternion.identity
        );

        DemonBindingCircleBehaviour behaviour =
            obj.GetComponent<DemonBindingCircleBehaviour>();

        if (behaviour != null)
        {
            behaviour.Initialize(user.transform, finalDuration, finalDamage);
        }
    }
}