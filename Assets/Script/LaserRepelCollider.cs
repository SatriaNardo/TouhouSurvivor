using UnityEngine;

public class LaserRepelCollider : MonoBehaviour
{
    public Transform player;

    public float repelForce = 3f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        if (player == null)
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy == null)
            return;

        Vector2 pushDir =
            (enemy.transform.position - player.position).normalized;

        enemy.transform.position +=
            (Vector3)(pushDir * repelForce * Time.deltaTime);
    }
}