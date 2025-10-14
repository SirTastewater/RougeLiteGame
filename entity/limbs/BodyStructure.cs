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
    
    public bool HasHead => heads.Count > 0;
    public bool HasArm => arms.Count > 0;
    public bool HasLeg => legs.Count > 0;
    private bool IsValid => torso != null;
    
    #region Public Body 
    public TorsoInstance Torso => torso;
    public Array<HeadInstance> Heads => heads;
    public Array<ArmInstance> Arms => arms;
    public Array<LegInstance> Legs => legs;
    #endregion
    
    
    public enum LimbAttribute { Strength, Speed, Life }

    /// <summary>
    /// Computes the total gain value of a specified limb attribute across all limbs of the body structure,
    /// including the torso, heads, arms, and legs.
    /// </summary>
    /// <param name="attribute">The limb attribute to compute the gain for. This can be Strength, Speed, or Life.</param>
    /// <returns>The total gain value of the specified attribute. Returns 0 if the body structure is not valid.</returns>
    public float ComputeGain(LimbAttribute attribute)
    {
        float value = 0;
        if (!IsValid) return value;
        
        Func<ILimbStats, float> func = ComputeAttributeFunction(attribute);

        value += func(torso);
        value += heads.Sum(func);
        value += arms.Sum(func);
        value += legs.Sum(func);
        
        return value;
    }

    /// <summary>
    /// Returns a function to compute the value of a specific limb attribute from a limb's stats.
    ///
    /// One must love functional programming
    /// </summary>
    /// <param name="attribute">The limb attribute to compute (e.g., Life, Strength, Speed).</param>
    /// <returns>A function that calculates the value of the specified attribute from an ILimbStats instance.</returns>
    private static Func<ILimbStats, float> ComputeAttributeFunction(LimbAttribute attribute)
    {
        return attribute switch
        {
            LimbAttribute.Life => limb => limb.Life,
            LimbAttribute.Strength => limb => limb.Strength,
            LimbAttribute.Speed => limb => limb.Speed,
            _ => _ => 0
        };
    }
}