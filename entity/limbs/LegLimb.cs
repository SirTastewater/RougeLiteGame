namespace RougeLiteGame.entity.limbs;

public partial class LegLimb : MirroredLimb
{
    public override LimbType GetLimbType()
    {
        return LimbType.Leg;
    }
}