using System;
using Godot;

namespace RougeLiteGame.entity.behavior;

// Imma just delete this later on
[GlobalClass] public partial class StatueIdleBehavior : IdleBehavior
{
    private Vector3 _targetPosition;
    private bool _isFacingTarget;

    protected override void Init()
    {
        SetTargetPosition(GenerateRandomPosition());
    }

    public override Vector3 Process(double delta)
    {
        if (!Entity.IsOnFloor()) return Entity.Velocity;

        if (GetTargetPosition() != Vector3.Zero && !Controller.IsNavigationFinished())
        {
            if (_isFacingTarget) return FollowControllerTarget();
            
            Vector3 target = GetTargetPosition() - Entity.GlobalPosition;
            target.Y = 0;
            target.Normalized();
                
            Vector3 facing = -Entity.GlobalTransform.Basis.Z;
            facing.Y = 0;
            facing = facing.Normalized();

            float angle = facing.AngleTo(target);

            if (angle < 0.01f)
            {
                _isFacingTarget = true;
                return FollowControllerTarget();
            }
                
            float cross = facing.Cross(target).Y;
            float turnDirection = float.Sign(cross);
            float rotationAmount = Mathf.Min(angle, 2.0f * (float) delta);
            
            Entity.Rotation = new Vector3(Entity.Rotation.X, Entity.Rotation.Y + turnDirection * rotationAmount, Entity.Rotation.Z);
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
        _isFacingTarget = false;
        
        return randomPosition;
    }
}