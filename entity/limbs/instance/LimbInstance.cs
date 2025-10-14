using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public abstract partial class LimbInstance<T> : Node, ILimbStats where T: Limb
{
    public T Limb => GetLimb();
    public bool IsValid => Limb != null;
    public bool IsDead => Life < 0;
    
    public float Life { get; private set; }

    public float Strength { get; private set; }

    public float Speed { get; private set; }

    protected abstract T GetLimb(); 

    public override void _Ready()
    {
        if (!IsValid) return;
        Life = Limb.LifeGain;
        Strength = Limb.StrengthGain;
        Speed = Limb.SpeedGain;
        
        base._Ready();
    }
}