using Godot;

namespace RougeLiteGame.entity.limbs.torso;

[GlobalClass] public partial class TorsoLimb : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Torso;
    }
}