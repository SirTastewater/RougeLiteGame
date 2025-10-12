namespace RougeLiteGame.entity.limbs;

public abstract partial class Arm : Limb
{
    protected override LimbType GetLimbType()
    {
        return LimbType.Arm;
    }
}