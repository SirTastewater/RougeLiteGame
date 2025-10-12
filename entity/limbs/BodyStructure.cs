using System;
using System.Linq;
using Godot.Collections;
using RougeLiteGame.entity.limbs.instance;

namespace RougeLiteGame.entity.limbs;

public readonly struct BodyStructure(
    
    TorsoInstance torso, 
    Array<HeadInstance> heads, 
    Array<ArmInstance> arms, 
    Array<LegInstance> legs
) {
    public bool HasHeads => heads.Count > 0;
    public bool HasArms => arms.Count > 0;
    public bool HasLegs => legs.Count > 0;

    public bool IsValid => torso != null;
    
    public enum LimbAttribute { Strength, Speed, Life }
    
    public float ComputeGain(LimbAttribute attribute)
    {
        if (!IsValid)
        {
            return 0;
        }
        
        return attribute switch
        {
            LimbAttribute.Strength => Compute(limb => limb.StrengthGain),
            LimbAttribute.Speed => Compute(limb => limb.SpeedGain),
            LimbAttribute.Life => Compute(limb => limb.LifeGain),
            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };
    }
    
    // TODO: use actual life values from the Limb
    private float Compute(Func<Limb, float> func)
    {
        float value = func.Invoke(torso.Limb);

        value += heads.Sum(headInstance => func.Invoke(headInstance.Limb));
        value += arms.Sum(armInstance => func.Invoke(armInstance.Limb));
        value += legs.Sum(legInstances => func.Invoke(legInstances.Limb));

        return value;
    }
}