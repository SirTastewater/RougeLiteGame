namespace RougeLiteGame.entity.limbs;

public partial class ArmLimb : MirroredLimb
{
    public override LimbType GetLimbType()
    {
        return LimbType.Arm;
    }
}