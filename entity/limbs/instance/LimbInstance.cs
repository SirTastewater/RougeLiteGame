using Godot;

namespace RougeLiteGame.entity.limbs.instance;

public partial class LimbInstance<T>(T limb) : Resource where T: Limb
{
    public T Limb => limb;
    public bool IsValid => limb != null;
    public bool IsDead => _life < 0;

    private float _life;
    
    public LimbInstance(T limb, float life) : this(limb)
    {
        _life = life;
    }

    public LimbInstance() : this(null) { }
}