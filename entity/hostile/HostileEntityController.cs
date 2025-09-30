using Godot;
using Godot.Collections;

namespace RougeLiteGame.entity.hostile;

[GlobalClass]
public partial class HostileEntityController : NavigationController
{
    [Export] private Array<Entity> _hostileTowards;
    private Entity _target;
    
    protected override Vector3 MovementProcess(double delta)
    {
        if (_hostileTowards.Count == 0) return Vector3.Zero;
        _target ??= _hostileTowards[0];
        
        SetTargetPosition(_target.GlobalPosition);
        return base.MovementProcess(delta);
    }
}