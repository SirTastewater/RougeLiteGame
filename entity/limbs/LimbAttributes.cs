using System;
using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass] public sealed partial class LimbAttributes : Resource
{ 
    public Limb Limb { get; private set; }
    private LimbType LimbType => Limb.GetLimbType();
    
    [Export(PropertyHint.Range, "-100,100,0.5")] private float _life, _strength, _speed;

    public void Initialize(Limb limb)
    {
        if (Limb != null)
        {
            throw new InvalidOperationException("The limb has already been initialized.");
        }

        Limb = limb;
        
        Init();
    }

    private void Init() { }

    public void Uninitialize()
    {
        if (Limb == null)
        {
            throw new InvalidOperationException("The limb has not been initialized.");
        }

        Limb = null;
    }
}