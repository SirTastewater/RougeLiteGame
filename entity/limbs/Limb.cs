using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public abstract partial class Limb : Node3D
{
    [Export(PropertyHint.Range, "-100,100,0.5")] private float _lifeGain, _strengthGain, _speedGain;
    
    public float LifeGain => _lifeGain;
    public float StrengthGain => _strengthGain;
    public float SpeedGain => _speedGain;
    
    public LimbType Type => GetLimbType();
    
    protected abstract LimbType GetLimbType();
}