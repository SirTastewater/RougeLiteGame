using System;
using System.Linq;
using Godot.Collections;

namespace RougeLiteGame.entity.limbs;

public readonly struct BodyStructure(Torso torso, Array<Head> heads, Array<Arm> arms, Array<Leg> legs)
{
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
            LimbAttribute.Strength => Compute(limb => limb.Strength),
            LimbAttribute.Speed => Compute(limb => limb.Speed),
            LimbAttribute.Life => Compute(limb => limb.Life),
            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };
    }
    
    private float Compute(Func<Limb, float> func)
    {
        float value = func.Invoke(torso);
        
        value += heads.Sum(func);
        value += arms.Sum(func);
        value += legs.Sum(func);
        
        return value;
    }
}