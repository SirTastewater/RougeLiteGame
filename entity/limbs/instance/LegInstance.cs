using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public partial class LegInstance : LimbInstance<Leg>
{
    [Export] private Leg _leg;
    
    protected override Leg GetLimb() { return _leg; }
}