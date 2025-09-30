using Godot;

namespace RougeLiteGame.entity;

public partial class NavigationController : EntityController
{
    
    [Export]
    private float _desiredDistance = 1.0f;
    
    protected override Vector3 MovementProcess(double delta)
    {
        if (!Entity.IsOnFloor()) return Entity.Velocity;
        
        // Do not query when the map has never synchronized and is empty.
        if (NavigationServer3D.MapGetIterationId(GetNavigationMap()) == 0) return Vector3.Zero;
        
        if(GetNextPathPosition() == Vector3.Zero) return Vector3.Zero;
        
        Vector3 nextPathPosition = GetNextPathPosition();
        if ((GetTargetPosition() - Entity.GlobalPosition).Length() < _desiredDistance)
        {
            return Vector3.Zero;
        }
        
        Vector3 direction = Entity.GlobalPosition.DirectionTo(nextPathPosition);
        Vector3 newVelocity = direction * MovementSpeed();

        Entity.LookAt(GetNextPathPosition());

        if (!AvoidanceEnabled) return newVelocity;
        Velocity = newVelocity;
        return newVelocity;
    }
}