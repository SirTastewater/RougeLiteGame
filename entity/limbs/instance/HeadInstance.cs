using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public partial class HeadInstance : LimbInstance<Head>
{
    [Export] private Head _head;
    
    protected override Head GetLimb() { return _head; }
}