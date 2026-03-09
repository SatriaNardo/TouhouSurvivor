public static class UltimateStatCalculator
{
    public static float GetStat(
        float baseValue,
        UltimateUpgradeType type,
        UltimateRuntimeStats runtime
    )
    {
        float value = baseValue;

        switch (type)
        {
            case UltimateUpgradeType.Duration:
                value += runtime.durationBonus;
                break;

            case UltimateUpgradeType.Damage:
                value += runtime.damageBonus;
                break;

            case UltimateUpgradeType.Cooldown:
                value -= runtime.cooldownReduction;
                break;

            case UltimateUpgradeType.SpeedMultiplier:
                value += runtime.speedMultiplierBonus;
                break;

            case UltimateUpgradeType.OrbCount:
                value += runtime.orbCountBonus;
                break;

            case UltimateUpgradeType.ExplosionRadius:
                value += runtime.explosionRadiusBonus;
                break;
        }

        return value;
    }
}