using UnityEngine;

public class StatsModifier
{
    public float value { get; private set; }
    public StatsModifierType modifierType { get; private set; }
    public StatsModifierSource modifierSource { get; private set; }

    public StatsModifier(float value, StatsModifierType modifierType, StatsModifierSource statsModifierSource)
    {
        this.value = value;
        this.modifierType = modifierType;
        this.modifierSource = modifierSource;
    }
    
    
}

public enum StatsModifierType { flatNum, percentage, percentMultiplier}

public enum StatsModifierSource {anting, skill, consummable}
