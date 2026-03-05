using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;

    [Header("Feedback")]
    public float flashDuration = 0.1f;

    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;

    private bool isKnockedBack;
    public bool IsKnockedBack => isKnockedBack;

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce, float knockbackDuration)
    {
        currentHealth -= damage;

        StartCoroutine(Flash());
        StartCoroutine(ApplyKnockback(hitDirection, knockbackForce, knockbackDuration));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ApplyKnockback(Vector2 direction, float force, float duration)
    {
        if (rb == null)
            yield break;

        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direction.normalized * force;

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
    }

    IEnumerator Flash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}