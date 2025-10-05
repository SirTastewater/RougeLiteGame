using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass]
public abstract partial class Limb : Node3D
{
    [Export] private LimbAttributes _limbAttributes;
    
    private LimbAttributes LimbAttributes
    {
        get => _limbAttributes;
        set { 
            _limbAttributes?.Uninitialize();
            _limbAttributes = value;
            _limbAttributes.Initialize(this);
        }
    }

    public override void _Ready()
    {
        // we must manually initialize as the editor does not call the setter at runtime
        // it's important that child classes still call base._Ready()
        _limbAttributes?.Initialize(this);
        base._Ready();
    }

    public abstract LimbType GetLimbType();
}