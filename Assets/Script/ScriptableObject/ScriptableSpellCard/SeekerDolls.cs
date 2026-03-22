using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Seeker Dolls")]
public class SeekerDolls : UltimateData
{
    public GameObject circlePrefab;
    public GameObject laserPrefab;

    public int circleCount = 8;
    public float radius = 2.5f;

    public float duration = 4f;

    public float damage = 20f;
    public float hitCooldown = 0.2f;

    public float coneAngle = 60f;
    public float sweepSpeed = 120f;

    public override void Activate(GameObject user)
    {
        SeekerDollsBehavior behavior =
            user.GetComponent<SeekerDollsBehavior>();

        if (behavior == null)
            behavior = user.AddComponent<SeekerDollsBehavior>();

        behavior.Activate(this, user);
    }
}