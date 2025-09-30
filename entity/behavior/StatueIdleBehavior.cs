using Godot;

namespace RougeLiteGame.entity.behavior;

// Imma just delete this later on
[GlobalClass] public partial class StatueIdleBehavior : IdleBehavior
{
    private Vector3 _targetPosition;
    private bool _isFacingTarget;
    
    public override Vector3 Process(double delta)
    {
        if (!Entity.IsOnFloor()) return Entity.Velocity;

        if (GetTargetPosition() != Vector3.Zero && !Controller.IsNavigationFinished()) return FollowControllerTarget();
        
        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(GD.RandRange(-10, 10), 1.4f, GD.RandRange(-10, 10));
        } while(randomPosition.DistanceTo(Entity.GlobalPosition) < 2);
            
        SetTargetPosition(randomPosition);
        Entity.LookAt(randomPosition);

        return FollowControllerTarget();
    }
}