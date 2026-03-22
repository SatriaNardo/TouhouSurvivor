using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Phantasmic Killer")]
public class PhantasmicKiller : UltimateData
{
    public GameObject knifePrefab;

    public int totalKnives = 100;
    public float spawnRate = 0.01f;

    public float speed = 10f;
    public float damage = 50f;
    public int bounceCount = 3;
    public float lifetime = 5f;

    public override void Activate(GameObject user)
    {
        PhantasmicKillerBehavior behavior = user.GetComponent<PhantasmicKillerBehavior>();

        if (behavior == null)
            behavior = user.AddComponent<PhantasmicKillerBehavior>();

        behavior.Activate(this, user);
    }
}