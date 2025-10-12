using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass] public partial class Leg : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Leg;
    }
}