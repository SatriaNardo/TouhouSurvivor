using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class NonDirectionalLaser : MonoBehaviour, IOrbitPiece
{
    private OrbitWeapon controller;
    private OrbitWeaponRuntimeStats stats;

    private Transform player;
    private float orbitAngle;

    private SpriteRenderer sprite;

    private bool canDealDamage = false;

    private float fadeInTime;
    private float activeTime;
    private float fadeOutTime;

    public void Initialize(OrbitWeaponRuntimeStats runtimeStats, Transform playerTransform, float startAngle)
    {
        stats = runtimeStats;
        player = playerTransform;
        orbitAngle = startAngle;

        controller = player.GetComponentInChildren<OrbitWeapon>();

        SetFadeStats(
            stats.fadeInTime,
            stats.activeTime,
            stats.fadeOutTime
        );
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        Color c = sprite.color;
        c.a = 0f;
        sprite.color = c;
    }

    void Update()
    {
        if (stats == null || player == null) return;

        HandleOrbit();
    }

    void HandleOrbit()
    {
        orbitAngle += stats.rotateSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        ) * stats.orbitDistance;

        transform.position = (Vector2)player.position + offset;

        // Optional: face outward
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetFadeStats(float fadeIn, float active, float fadeOut)
    {
        fadeInTime = fadeIn;
        activeTime = active;
        fadeOutTime = fadeOut;

        StopAllCoroutines();
        StartCoroutine(FadeLoop());
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            canDealDamage = false;
            yield return Fade(0f, 1f, fadeInTime);

            canDealDamage = true;
            yield return new WaitForSeconds(activeTime);

            canDealDamage = false;
            yield return Fade(1f, 0f, fadeOutTime);
        }
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float alpha = Mathf.Lerp(from, to, t);

            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;

            yield return null;
        }

        Color final = sprite.color;
        final.a = to;
        sprite.color = final;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!canDealDamage)
            return;

        controller?.HandleDamage(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        controller?.HandleExit(other);
    }
}