using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Laser Ultimate")]
public class LaserUltimate : UltimateData
{
    public GameObject laserPrefab;

    public float duration = 3f;
    public float damage = 30f;
    public float hitCooldown = 0.3f;

    public float distanceFromPlayer = 1.5f;

    public override void Activate(GameObject user)
    {
        if (laserPrefab == null || user == null)
            return;

        // Spawn as CHILD of player
        GameObject obj = Instantiate(laserPrefab, user.transform.position, Quaternion.identity, user.transform);

        LaserUltimateBehaviour laser =
            obj.GetComponent<LaserUltimateBehaviour>();

        if (laser != null)
        {
            laser.Initialize(user.transform, duration,damage,hitCooldown,distanceFromPlayer);
        }
    }
}