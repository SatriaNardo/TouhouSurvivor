using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class NonDirectionalLaser : MonoBehaviour
{
    private OrbitWeapon controller;
    private SpriteRenderer sprite;

    private bool canDealDamage = false;

    private float fadeInTime;
    private float activeTime;
    private float fadeOutTime;

    public void SetController(OrbitWeapon orbitWeapon)
    {
        controller = orbitWeapon;
    }

    public void SetFadeStats(float fadeIn, float active, float fadeOut)
    {
        fadeInTime = fadeIn;
        activeTime = active;
        fadeOutTime = fadeOut;

        StopAllCoroutines();
        StartCoroutine(FadeLoop());
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        Color c = sprite.color;
        c.a = 0f;
        sprite.color = c;
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            // FADE IN
            canDealDamage = false;
            yield return Fade(0f, 1f, fadeInTime);

            // ACTIVE
            canDealDamage = true;
            yield return new WaitForSeconds(activeTime);

            // FADE OUT
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