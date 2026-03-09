using System.Collections.Generic;
using UnityEngine;

public class DemonBindingCircleBehaviour : MonoBehaviour
{
    private Transform player;
    private PlayerController playerController;
    
    private float duration;
    private float damage;

    private float timer;

    private Vector3 startScale = Vector3.one * 0.2f;
    private Vector3 targetScale;

    private BoxCollider2D boxCollider;

    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void Initialize(Transform playerTransform, float dur, float dmg)
    {
        player = playerTransform;

        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        duration = dur;
        damage = dmg;

        timer = duration;

        transform.SetParent(player);
        transform.localPosition = Vector3.zero;

        transform.localScale = startScale;

        boxCollider = GetComponent<BoxCollider2D>();

        CalculateScreenScale();
    }

    void CalculateScreenScale()
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        Vector3 scale = new Vector3(width, height, 1f);

        if (player != null)
        {
            Vector3 parentScale = player.lossyScale;

            scale.x /= parentScale.x;
            scale.y /= parentScale.y;
        }

        // Make it 90% of the screen
        scale *= 0.8f;

        targetScale = scale;
    }
    void Update()
    {
        timer -= Time.deltaTime;

        float t = 1f - (timer / duration);
        t = Mathf.Pow(t, 0.5f);

        transform.localScale = Vector3.Lerp((startScale), targetScale, t);

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (hitEnemies.Contains(enemy))
                return;

            hitEnemies.Add(enemy);

            Vector2 hitDir = (enemy.transform.position - transform.position).normalized;

            enemy.TakeDamage((int)damage, hitDir, 8f, 0.3f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.magenta;

        if (boxCollider != null)
            Gizmos.DrawWireCube(transform.position, boxCollider.size * transform.localScale);
    }
}