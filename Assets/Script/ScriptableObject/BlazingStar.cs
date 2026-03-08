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
                duration,
                damage,
                hitCooldown,
                speedMultiplier
            );
        }
    }
}