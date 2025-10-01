using System;
using Godot;

namespace RougeLiteGame.entity.behavior;

// Imma just delete this later on
[GlobalClass] public partial class StatueIdleBehavior : IdleBehavior
{
    private Vector3 _targetPosition;

    protected override void Init()
    {
        SetTargetPosition(GenerateRandomPosition());
    }

    public override Vector3 Process(double delta)
    {
        if (!Entity.IsOnFloor()) return Entity.Velocity;

        if (GetTargetPosition() != Vector3.Zero && !Controller.IsNavigationFinished())
        {
            if (IsFacingTarget()) return FollowControllerTarget();
            
            RotateTowards(delta);
            return Vector3.Zero;
        }
        
        SetTargetPosition(GenerateRandomPosition());
        return FollowControllerTarget();
    }

    private Vector3 GenerateRandomPosition()
    {
        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(GD.RandRange(-10, 10), 1.4f, GD.RandRange(-10, 10));
        } while(randomPosition.DistanceTo(Entity.GlobalPosition) < 4);
            
        SetTargetPosition(randomPosition);
        
        return randomPosition;
    }
}