using UnityEngine;

[CreateAssetMenu(menuName = "Ultimate/Sakuya World")]
public class SakuyaWorld : UltimateData
{
    public float duration = 4f;

    public override void Activate(GameObject user)
    {
        if (SakuyaWorldBehaviour.Instance == null)
        {
            new GameObject("TimeStopManager")
                .AddComponent<SakuyaWorldBehaviour>();
        }

        SakuyaWorldBehaviour.Instance.StartTimeStop(duration);
    }
}