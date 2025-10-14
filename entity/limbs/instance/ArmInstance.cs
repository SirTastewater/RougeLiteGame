using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public partial class ArmInstance : LimbInstance<Arm>
{
    [Export] private Arm _arm;
    
    protected override Arm GetLimb() { return _arm; }
}