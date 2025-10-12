using Godot;

namespace RougeLiteGame.entity.limbs;

[GlobalClass] public partial class Head : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Head;
    }
}