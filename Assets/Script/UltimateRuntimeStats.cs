using System;

[Serializable]
public class UltimateRuntimeStats
{
    public float durationBonus;
    public float damageBonus;
    public float cooldownReduction;
    public float speedMultiplierBonus;
    public int orbCountBonus;
    public float explosionRadiusBonus;

    public void ApplyUpgrade(UltimateUpgradeType type, float value)
    {
        switch (type)
        {
            case UltimateUpgradeType.Duration:
                durationBonus += value;
                break;

            case UltimateUpgradeType.Damage:
                damageBonus += value;
                break;

            case UltimateUpgradeType.Cooldown:
                cooldownReduction += value;
                break;

            case UltimateUpgradeType.SpeedMultiplier:
                speedMultiplierBonus += value;
                break;

            case UltimateUpgradeType.OrbCount:
                orbCountBonus += (int)value;
                break;

            case UltimateUpgradeType.ExplosionRadius:
                explosionRadiusBonus += value;
                break;
        }
    }
}