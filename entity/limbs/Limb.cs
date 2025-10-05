using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public abstract partial class Limb : Node3D
{
    [Export(PropertyHint.Range, "-100,100,0.5")] private float _life, _strength, _speed;
    
    public float Life => _life;
    public float Strength => _strength;
    public float Speed => _speed;
    
    public LimbType Type => GetLimbType();
    
    protected abstract LimbType GetLimbType();
}