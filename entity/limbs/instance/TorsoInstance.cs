using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public partial class TorsoInstance : LimbInstance<Torso>
{
    [Export] private Torso _torso;
    
    protected override Torso GetLimb() { return _torso; }
}