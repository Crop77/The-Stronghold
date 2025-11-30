using UnityEngine;

public enum DamageType
{
    Melee,
    Ranged,
    Siege
}

public enum ArmorType
{
    Light,
    Heavy,
    Fortified
}

public static class CombatCalculator
{
    public static int CalculateDamage(int baseDamage, DamageType damageType, ArmorType armorType)
    {
        float multiplier = GetMultiplier(damageType, armorType);
        int final = Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier));
        return final;
    }

    static float GetMultiplier(DamageType dmgType, ArmorType armor)
    {
        // Melee:  Effective vs Heavy, Neutral vs Light, Weak vs Fortified
        // Ranged: Effective vs Light, Neutral vs Fortified, Weak vs Heavy
        // Siege:  Effective vs Fortified, Neutral vs Heavy, Weak vs Light

        switch (dmgType)
        {
            case DamageType.Melee:
                switch (armor)
                {
                    case ArmorType.Heavy: return 1.25f; 
                    case ArmorType.Fortified: return 0.75f; 
                    case ArmorType.Light:
                    default: return 1.0f;  
                }

            case DamageType.Ranged:
                switch (armor)
                {
                    case ArmorType.Light: return 1.25f; 
                    case ArmorType.Heavy: return 0.75f; 
                    case ArmorType.Fortified:
                    default: return 1.0f;  
                }

            case DamageType.Siege:
                switch (armor)
                {
                    case ArmorType.Fortified: return 1.5f;  
                    case ArmorType.Light: return 0.5f;  
                    case ArmorType.Heavy:
                    default: return 1.0f;  
                }

            default:
                return 1.0f;
        }
    }
}
