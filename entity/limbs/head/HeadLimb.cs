using Godot;

namespace RougeLiteGame.entity.limbs.head;

[GlobalClass] public partial class HeadLimb : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Head;
    }
}