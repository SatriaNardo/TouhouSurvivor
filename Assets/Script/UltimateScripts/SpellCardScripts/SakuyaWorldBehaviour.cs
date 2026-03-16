using UnityEngine;

public class SakuyaWorldBehaviour : MonoBehaviour
{
    public static SakuyaWorldBehaviour Instance;

    public bool IsTimeStopped { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void StartTimeStop(float duration)
    {
        if (!IsTimeStopped)
            StartCoroutine(TimeStopRoutine(duration));
    }

    System.Collections.IEnumerator TimeStopRoutine(float duration)
    {
        IsTimeStopped = true;

        yield return new WaitForSecondsRealtime(duration);

        IsTimeStopped = false;
    }
}