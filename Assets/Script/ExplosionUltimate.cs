using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Explosion")]
public class ExplosionUltimate : UltimateData
{
    public float radius = 5f;
    public int damage = 50;

    public override void Activate(GameObject user)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            user.transform.position,
            radius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                if (enemy != null)
                {
                    Vector2 dir =
                        (enemy.transform.position -
                         user.transform.position);

                    enemy.TakeDamage(
                        damage,
                        dir,
                        5f,
                        0.2f
                    );
                }
            }
        }
    }
}