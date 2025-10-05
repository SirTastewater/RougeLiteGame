using Godot;

namespace RougeLiteGame.entity.limbs.leg;

[GlobalClass] public partial class LegLimb : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Leg;
    }
}