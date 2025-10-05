namespace RougeLiteGame.entity.limbs.arm;

public abstract partial class ArmLimb : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Arm;
    }
}