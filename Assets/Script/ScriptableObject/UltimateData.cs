using UnityEngine;

public abstract class UltimateData : ScriptableObject
{
    [Header("Ultimate Info")]
    public string ultimateName;
    public float cooldown = 10f;

    [Header("Upgrades")]
    public UltimateUpgrade[] upgrades;

    public abstract void Activate(GameObject user);
}